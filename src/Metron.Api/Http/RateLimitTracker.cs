using System.Net.Http.Headers;

namespace Metron.Api.Http;

/// <summary>
/// Thread-safe holder for the most recently observed <see cref="RateLimitStatus"/>, used to
/// throttle proactively before either counter is actually exhausted.
/// </summary>
public sealed class RateLimitTracker
{
    private readonly object _gate = new();
    private RateLimitStatus? _status;

    public RateLimitStatus? Current
    {
        get
        {
            lock (_gate)
            {
                return _status;
            }
        }
    }

    public void Update(HttpResponseHeaders headers)
    {
        var status = RateLimitStatus.FromHeaders(headers);
        if (status is null)
        {
            return;
        }

        lock (_gate)
        {
            _status = status;
        }
    }

    /// <summary>
    /// Delays until the burst/sustained reset time if the last known response reported either
    /// counter at zero, so the next request doesn't get thrown away as a 429.
    /// </summary>
    public async Task WaitIfNeededAsync(TimeProvider timeProvider, CancellationToken cancellationToken)
    {
        var status = Current;
        if (status is null)
        {
            return;
        }

        var now = timeProvider.GetUtcNow();
        var wait = TimeSpan.Zero;

        if (status.BurstRemaining == 0 && status.BurstReset is { } burstReset && burstReset > now)
        {
            wait = burstReset - now;
        }

        if (status.SustainedRemaining == 0 && status.SustainedReset is { } sustainedReset && sustainedReset > now)
        {
            var sustainedWait = sustainedReset - now;
            if (sustainedWait > wait)
            {
                wait = sustainedWait;
            }
        }

        if (wait > TimeSpan.Zero)
        {
            await Task.Delay(wait, timeProvider, cancellationToken).ConfigureAwait(false);
        }
    }
}
