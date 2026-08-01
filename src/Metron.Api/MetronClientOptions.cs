namespace Metron.Api;

/// <summary>Options for constructing a <see cref="MetronClient"/>.</summary>
public sealed class MetronClientOptions
{
    /// <summary>Metron API token, sent as "Authorization: Bearer &lt;ApiToken&gt;".</summary>
    public required string ApiToken { get; init; }

    /// <summary>Root URL of the API. Override for testing against a different host.</summary>
    public Uri BaseAddress { get; init; } = new("https://metron.cloud/api/");

    /// <summary>
    /// Sent as the User-Agent header. The API docs ask clients to identify themselves; override
    /// this with your application's name.
    /// </summary>
    public string UserAgent { get; init; } = "Metron.Api-dotnet/1.0";

    /// <summary>How many times to retry a request that comes back 429, honoring Retry-After each time.</summary>
    public int MaxRetryAttempts { get; init; } = 3;

    /// <summary>
    /// Optional innermost transport handler (e.g. a fake handler for tests). Defaults to a new
    /// <see cref="HttpClientHandler"/>. <see cref="MetronClient"/> always layers its own
    /// auth + <see cref="Http.RateLimitHandler"/> on top of this handler.
    /// </summary>
    public HttpMessageHandler? TransportHandler { get; init; }
}
