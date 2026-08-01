using System.Net;
using System.Text.Json;
using Metron.Api.Exceptions;

namespace Metron.Api.Http;

/// <summary>
/// DelegatingHandler implementing the client behavior recommended by
/// https://github.com/Metron-Project/metron/blob/master/api/RATELIMIT.md:
///   1. Proactively wait if the last known burst/sustained counter was exhausted, instead of
///      firing a request that's guaranteed to be throttled.
///   2. Track the six X-RateLimit-* headers from every response.
///   3. On an actual 429, honor Retry-After and retry (bounded), rather than using a fixed or
///      exponential backoff -- exponential backoff is reserved for network/5xx errors, which
///      this handler does not touch.
/// </summary>
public sealed class RateLimitHandler : DelegatingHandler
{
    private readonly RateLimitTracker _tracker;
    private readonly int _maxRetryAttempts;
    private readonly TimeProvider _timeProvider;

    /// <summary>Creates a handler that shares <paramref name="tracker"/>'s state across every request it sends.</summary>
    public RateLimitHandler(RateLimitTracker tracker, int maxRetryAttempts, TimeProvider? timeProvider = null)
    {
        _tracker = tracker;
        _maxRetryAttempts = maxRetryAttempts;
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        for (var attempt = 0; ; attempt++)
        {
            await _tracker.WaitIfNeededAsync(_timeProvider, cancellationToken).ConfigureAwait(false);

            // The original request can only be sent once; retries need a fresh copy. Note this
            // requires request content to be re-readable (true for the StringContent/
            // MultipartFormDataContent produced by MultipartFormDataBuilder, but NOT for a
            // one-shot, non-seekable Stream if a caller attaches one manually).
            var requestToSend = attempt == 0 ? request : await CloneAsync(request).ConfigureAwait(false);
            var response = await base.SendAsync(requestToSend, cancellationToken).ConfigureAwait(false);
            _tracker.Update(response.Headers);

            if (response.StatusCode != HttpStatusCode.TooManyRequests)
            {
                return response;
            }

            var retryAfter = GetRetryAfter(response) ?? TimeSpan.FromSeconds(1);

            if (attempt >= _maxRetryAttempts)
            {
                var detail = await TryReadDetailAsync(response, cancellationToken).ConfigureAwait(false);
                response.Dispose();
                throw new MetronRateLimitException(retryAfter, detail, _tracker.Current);
            }

            response.Dispose();
            await Task.Delay(retryAfter, _timeProvider, cancellationToken).ConfigureAwait(false);
        }
    }

    private static TimeSpan? GetRetryAfter(HttpResponseMessage response)
    {
        var retryAfter = response.Headers.RetryAfter;
        if (retryAfter is null)
        {
            return null;
        }

        if (retryAfter.Delta is { } delta)
        {
            return delta;
        }

        if (retryAfter.Date is { } date)
        {
            var wait = date - DateTimeOffset.UtcNow;
            return wait > TimeSpan.Zero ? wait : TimeSpan.Zero;
        }

        return null;
    }

    private static async Task<string?> TryReadDetailAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (doc.RootElement.ValueKind == JsonValueKind.Object && doc.RootElement.TryGetProperty("detail", out var detail))
            {
                return detail.GetString();
            }
        }
        catch (JsonException)
        {
            // Body wasn't JSON (or was empty) -- fall through with no detail.
        }

        return null;
    }

    private static async Task<HttpRequestMessage> CloneAsync(HttpRequestMessage request)
    {
        var clone = new HttpRequestMessage(request.Method, request.RequestUri)
        {
            Version = request.Version,
        };

        foreach (var header in request.Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        if (request.Content is not null)
        {
            var buffer = await request.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var content = new ByteArrayContent(buffer);
            foreach (var header in request.Content.Headers)
            {
                content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            clone.Content = content;
        }

        foreach (var option in request.Options)
        {
            clone.Options.TryAdd(option.Key, option.Value);
        }

        return clone;
    }
}
