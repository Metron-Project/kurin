using System.Linq;
using System.Net.Http.Headers;

namespace Metron.Api.Http;

/// <summary>
/// A snapshot of the six X-RateLimit-* headers the Metron API returns on every response.
/// See https://github.com/Metron-Project/metron/blob/master/api/RATELIMIT.md.
/// </summary>
/// <param name="BurstLimit">Total requests allowed in the current 1-minute burst window.</param>
/// <param name="BurstRemaining">Requests left in the current burst window.</param>
/// <param name="BurstReset">When the burst window's counter resets.</param>
/// <param name="SustainedLimit">Total requests allowed in the current 1-day sustained window.</param>
/// <param name="SustainedRemaining">Requests left in the current sustained window.</param>
/// <param name="SustainedReset">When the sustained window's counter resets.</param>
public sealed record RateLimitStatus(
    int? BurstLimit,
    int? BurstRemaining,
    DateTimeOffset? BurstReset,
    int? SustainedLimit,
    int? SustainedRemaining,
    DateTimeOffset? SustainedReset)
{
    /// <summary>Parses the six X-RateLimit-* headers from a response, or null if none are present.</summary>
    public static RateLimitStatus? FromHeaders(HttpResponseHeaders headers)
    {
        var burstLimit = GetInt(headers, "X-RateLimit-Burst-Limit");
        var burstRemaining = GetInt(headers, "X-RateLimit-Burst-Remaining");
        var burstReset = GetUnixSeconds(headers, "X-RateLimit-Burst-Reset");
        var sustainedLimit = GetInt(headers, "X-RateLimit-Sustained-Limit");
        var sustainedRemaining = GetInt(headers, "X-RateLimit-Sustained-Remaining");
        var sustainedReset = GetUnixSeconds(headers, "X-RateLimit-Sustained-Reset");

        if (burstLimit is null && sustainedLimit is null)
        {
            return null;
        }

        return new RateLimitStatus(burstLimit, burstRemaining, burstReset, sustainedLimit, sustainedRemaining, sustainedReset);
    }

    private static int? GetInt(HttpResponseHeaders headers, string name) =>
        headers.TryGetValues(name, out var values) && int.TryParse(values.FirstOrDefault(), out var value)
            ? value
            : null;

    private static DateTimeOffset? GetUnixSeconds(HttpResponseHeaders headers, string name) =>
        headers.TryGetValues(name, out var values) && long.TryParse(values.FirstOrDefault(), out var value)
            ? DateTimeOffset.FromUnixTimeSeconds(value)
            : null;
}
