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

    public ArcsClient Arc { get; }
    public CharactersClient Character { get; }
    public CollectionClient Collection { get; }
    public CreatorsClient Creator { get; }
    public CreditsClient Credit { get; }
    public ImprintsClient Imprint { get; }
    public IssuesClient Issue { get; }
    public PublishersClient Publisher { get; }
    public PullListClient PullList { get; }
    public ReadingListClient ReadingList { get; }
    public RolesClient Role { get; }
    public SeriesClient Series { get; }
    public SeriesTypesClient SeriesType { get; }
    public TeamsClient Team { get; }
    public UniversesClient Universe { get; }
    public VariantsClient Variant { get; }
    public WishListClient WishList { get; }

    public void Dispose() => _http.Dispose();
}
