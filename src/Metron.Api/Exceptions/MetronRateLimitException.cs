using System.Net;
using Metron.Api.Http;

namespace Metron.Api.Exceptions;

/// <summary>
/// Thrown when a request is still throttled (429) after <see cref="MetronClientOptions.MaxRetryAttempts"/>
/// retries, each of which already honored the API's Retry-After header.
/// </summary>
public sealed class MetronRateLimitException : MetronApiException
{
    /// <summary>The Retry-After duration from the final 429 response.</summary>
    public TimeSpan RetryAfter { get; }

    /// <summary>The rate-limit snapshot from the final 429 response, if headers were present.</summary>
    public RateLimitStatus? RateLimitStatus { get; }

    public MetronRateLimitException(TimeSpan retryAfter, string? detail, RateLimitStatus? rateLimitStatus)
        : base(HttpStatusCode.TooManyRequests, detail)
    {
        RetryAfter = retryAfter;
        RateLimitStatus = rateLimitStatus;
    }
}
