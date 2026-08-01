using System.Net;
using System.Net.Http.Headers;
using Metron.Api.Exceptions;
using Metron.Api.Http;
using Microsoft.Extensions.Time.Testing;

namespace Metron.Api.Tests;

public class RateLimitHandlerTests
{
    private static HttpResponseMessage OkResponseWithHeaders(
        int burstLimit, int burstRemaining, DateTimeOffset burstReset,
        int sustainedLimit, int sustainedRemaining, DateTimeOffset sustainedReset)
    {
        var response = FakeHttpMessageHandler.Json(HttpStatusCode.OK, "{}");
        response.Headers.Add("X-RateLimit-Burst-Limit", burstLimit.ToString());
        response.Headers.Add("X-RateLimit-Burst-Remaining", burstRemaining.ToString());
        response.Headers.Add("X-RateLimit-Burst-Reset", burstReset.ToUnixTimeSeconds().ToString());
        response.Headers.Add("X-RateLimit-Sustained-Limit", sustainedLimit.ToString());
        response.Headers.Add("X-RateLimit-Sustained-Remaining", sustainedRemaining.ToString());
        response.Headers.Add("X-RateLimit-Sustained-Reset", sustainedReset.ToUnixTimeSeconds().ToString());
        return response;
    }

    [Fact]
    public async Task SendAsync_WaitsProactively_WhenBurstRemainingIsZero()
    {
        var timeProvider = new FakeTimeProvider();
        var tracker = new RateLimitTracker();
        // Prime the tracker as if a prior response reported the burst counter exhausted.
        var primerHeaders = OkResponseWithHeaders(20, 0, timeProvider.GetUtcNow().AddSeconds(30), 5000, 4000, timeProvider.GetUtcNow().AddDays(1));
        tracker.Update(primerHeaders.Headers);

        var fake = new FakeHttpMessageHandler();
        fake.Enqueue(FakeHttpMessageHandler.Json(HttpStatusCode.OK, "{}"));
        var handler = new RateLimitHandler(tracker, maxRetryAttempts: 3, timeProvider) { InnerHandler = fake };
        using var invoker = new HttpMessageInvoker(handler);

        var sendTask = invoker.SendAsync(new HttpRequestMessage(HttpMethod.Get, "https://example.test/x"), CancellationToken.None);

        // The request must not reach the inner handler until the burst window resets.
        await Task.Delay(50);
        Assert.Empty(fake.Requests);

        timeProvider.Advance(TimeSpan.FromSeconds(30));
        var response = await sendTask;

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Single(fake.Requests);
    }

    [Fact]
    public async Task SendAsync_Honors429RetryAfter_ThenSucceeds()
    {
        var timeProvider = new FakeTimeProvider();
        var tracker = new RateLimitTracker();

        var fake = new FakeHttpMessageHandler();
        fake.Enqueue(_ =>
        {
            var throttled = new HttpResponseMessage(HttpStatusCode.TooManyRequests)
            {
                Content = new StringContent("{\"detail\":\"Request was throttled. Expected available in 5 seconds.\"}"),
            };
            throttled.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromSeconds(5));
            return throttled;
        });
        fake.Enqueue(FakeHttpMessageHandler.Json(HttpStatusCode.OK, "{\"ok\":true}"));

        var handler = new RateLimitHandler(tracker, maxRetryAttempts: 3, timeProvider) { InnerHandler = fake };
        using var invoker = new HttpMessageInvoker(handler);

        var sendTask = invoker.SendAsync(
            new HttpRequestMessage(HttpMethod.Get, "https://example.test/x"), CancellationToken.None);

        // Let the first (429) attempt land, then advance past its Retry-After.
        await WaitForRequestCountAsync(fake, 1);
        timeProvider.Advance(TimeSpan.FromSeconds(5));

        var response = await sendTask;

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, fake.Requests.Count);
    }

    [Fact]
    public async Task SendAsync_ThrowsMetronRateLimitException_AfterExhaustingRetries()
    {
        var timeProvider = new FakeTimeProvider();
        var tracker = new RateLimitTracker();

        var fake = new FakeHttpMessageHandler();
        for (var i = 0; i < 2; i++)
        {
            fake.Enqueue(_ =>
            {
                var throttled = new HttpResponseMessage(HttpStatusCode.TooManyRequests)
                {
                    Content = new StringContent("{\"detail\":\"still throttled\"}"),
                };
                throttled.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromSeconds(1));
                return throttled;
            });
        }

        var handler = new RateLimitHandler(tracker, maxRetryAttempts: 1, timeProvider) { InnerHandler = fake };
        using var invoker = new HttpMessageInvoker(handler);

        var sendTask = invoker.SendAsync(
            new HttpRequestMessage(HttpMethod.Get, "https://example.test/x"), CancellationToken.None);

        await WaitForRequestCountAsync(fake, 1);
        timeProvider.Advance(TimeSpan.FromSeconds(1));

        var exception = await Assert.ThrowsAsync<MetronRateLimitException>(() => sendTask);
        Assert.Equal(TimeSpan.FromSeconds(1), exception.RetryAfter);
        Assert.Equal("still throttled", exception.Detail);
        Assert.Equal(2, fake.Requests.Count);
    }

    [Fact]
    public async Task SendAsync_PassesThroughNon429Errors_Untouched()
    {
        var timeProvider = new FakeTimeProvider();
        var tracker = new RateLimitTracker();

        var fake = new FakeHttpMessageHandler();
        fake.Enqueue(FakeHttpMessageHandler.Json(HttpStatusCode.NotFound, "{\"detail\":\"Not found.\"}"));

        var handler = new RateLimitHandler(tracker, maxRetryAttempts: 3, timeProvider) { InnerHandler = fake };
        using var invoker = new HttpMessageInvoker(handler);

        var response = await invoker.SendAsync(
            new HttpRequestMessage(HttpMethod.Get, "https://example.test/x"), CancellationToken.None);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Single(fake.Requests);
    }

    private static async Task WaitForRequestCountAsync(FakeHttpMessageHandler fake, int count)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        while (fake.Requests.Count < count)
        {
            cts.Token.ThrowIfCancellationRequested();
            await Task.Delay(5, CancellationToken.None);
        }
    }
}
