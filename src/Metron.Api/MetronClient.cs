using System.Net.Http.Headers;
using System.Text.Json;
using Metron.Api.Http;
using Metron.Api.Resources;

namespace Metron.Api;

/// <summary>
/// Entry point for the Metron API client. Owns a single HttpClient shared by every resource
/// client, with rate-limit tracking/throttling (see <see cref="Http.RateLimitHandler"/>) and
/// bearer-token auth wired in automatically.
/// </summary>
public sealed class MetronClient : IDisposable
{
    private readonly HttpClient _http;
    private readonly RateLimitTracker _tracker = new();

    /// <summary>Creates a client configured from <paramref name="options"/>.</summary>
    public MetronClient(MetronClientOptions options)
    {
        var transport = options.TransportHandler ?? new HttpClientHandler();
        var rateLimitHandler = new RateLimitHandler(_tracker, options.MaxRetryAttempts) { InnerHandler = transport };

        _http = new HttpClient(rateLimitHandler) { BaseAddress = options.BaseAddress };
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.ApiToken);
        _http.DefaultRequestHeaders.UserAgent.ParseAdd(options.UserAgent);
        _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        Arc = new ArcsClient(_http, jsonOptions);
        Character = new CharactersClient(_http, jsonOptions);
        Collection = new CollectionClient(_http, jsonOptions);
        Creator = new CreatorsClient(_http, jsonOptions);
        Credit = new CreditsClient(_http, jsonOptions);
        Imprint = new ImprintsClient(_http, jsonOptions);
        Issue = new IssuesClient(_http, jsonOptions);
        Publisher = new PublishersClient(_http, jsonOptions);
        PullList = new PullListClient(_http, jsonOptions);
        ReadingList = new ReadingListClient(_http, jsonOptions);
        Role = new RolesClient(_http, jsonOptions);
        Series = new SeriesClient(_http, jsonOptions);
        SeriesType = new SeriesTypesClient(_http, jsonOptions);
        Team = new TeamsClient(_http, jsonOptions);
        Universe = new UniversesClient(_http, jsonOptions);
        Variant = new VariantsClient(_http, jsonOptions);
        WishList = new WishListClient(_http, jsonOptions);
    }

    /// <summary>The most recently observed rate-limit snapshot, updated after every response.</summary>
    public RateLimitStatus? RateLimitStatus => _tracker.Current;

    /// <summary>Story arcs.</summary>
    public ArcsClient Arc { get; }

    /// <summary>Comic characters.</summary>
    public CharactersClient Character { get; }

    /// <summary>The authenticated user's comic collection.</summary>
    public CollectionClient Collection { get; }

    /// <summary>Comic creators.</summary>
    public CreatorsClient Creator { get; }

    /// <summary>Credits linking a creator to an issue with a role.</summary>
    public CreditsClient Credit { get; }

    /// <summary>Publisher imprints.</summary>
    public ImprintsClient Imprint { get; }

    /// <summary>Individual comic issues.</summary>
    public IssuesClient Issue { get; }

    /// <summary>Comic publishers.</summary>
    public PublishersClient Publisher { get; }

    /// <summary>The authenticated user's pull list.</summary>
    public PullListClient PullList { get; }

    /// <summary>Reading lists (read-only).</summary>
    public ReadingListClient ReadingList { get; }

    /// <summary>Creator roles (read-only).</summary>
    public RolesClient Role { get; }

    /// <summary>Comic series.</summary>
    public SeriesClient Series { get; }

    /// <summary>Series types (read-only).</summary>
    public SeriesTypesClient SeriesType { get; }

    /// <summary>Teams of characters.</summary>
    public TeamsClient Team { get; }

    /// <summary>Fictional universes.</summary>
    public UniversesClient Universe { get; }

    /// <summary>Variant covers.</summary>
    public VariantsClient Variant { get; }

    /// <summary>The authenticated user's wish list.</summary>
    public WishListClient WishList { get; }

    /// <summary>Disposes the underlying HttpClient.</summary>
    public void Dispose() => _http.Dispose();
}
