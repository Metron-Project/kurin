using System.Text.Json;
using Metron.Api.Filters;
using Metron.Api.Http;
using Metron.Api.Models;

namespace Metron.Api.Resources;

/// <summary>The authenticated user's comic collection. Requires authentication.</summary>
public sealed class CollectionClient(HttpClient http, JsonSerializerOptions jsonOptions) : ResourceClientBase(http, jsonOptions)
{
    public Task<PagedResult<CollectionList>> ListAsync(CollectionFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<CollectionList>>("collection/" + QueryStringBuilder.Build(filter), cancellationToken);

    public IAsyncEnumerable<CollectionList> ListAllAsync(CollectionFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<CollectionList>("collection/" + QueryStringBuilder.Build(filter), cancellationToken);

    public Task<CollectionRead> GetAsync(int id, CancellationToken cancellationToken = default) =>
        GetAsync<CollectionRead>($"collection/{id}/", cancellationToken);

    /// <summary>Updates the rating of a collection item. Read-tracking is handled via <see cref="ScrobbleAsync"/> instead.</summary>
    public Task<CollectionRatingUpdate> UpdateAsync(int id, CollectionRatingUpdate rating, CancellationToken cancellationToken = default) =>
        SendFormAsync<CollectionRatingUpdate>(HttpMethod.Put, $"collection/{id}/", rating, cancellationToken);

    public Task<CollectionRatingUpdate> PartialUpdateAsync(int id, PatchedCollectionRatingUpdate rating, CancellationToken cancellationToken = default) =>
        SendFormAsync<CollectionRatingUpdate>(HttpMethod.Patch, $"collection/{id}/", rating, cancellationToken);

    public Task<PagedResult<MissingIssue>> GetMissingIssuesAsync(int seriesId, CollectionMissingIssuesFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<MissingIssue>>($"collection/missing_issues/{seriesId}/" + QueryStringBuilder.Build(filter), cancellationToken);

    public IAsyncEnumerable<MissingIssue> GetMissingIssuesAllAsync(int seriesId, CollectionMissingIssuesFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<MissingIssue>($"collection/missing_issues/{seriesId}/" + QueryStringBuilder.Build(filter), cancellationToken);

    public Task<PagedResult<MissingSeries>> GetMissingSeriesAsync(CollectionMissingSeriesFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<MissingSeries>>("collection/missing_series/" + QueryStringBuilder.Build(filter), cancellationToken);

    public IAsyncEnumerable<MissingSeries> GetMissingSeriesAllAsync(CollectionMissingSeriesFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<MissingSeries>("collection/missing_series/" + QueryStringBuilder.Build(filter), cancellationToken);

    /// <summary>Marks an issue as read. Auto-creates the collection item if needed.</summary>
    public Task<ScrobbleResponse> ScrobbleAsync(ScrobbleRequest request, CancellationToken cancellationToken = default) =>
        SendFormAsync<ScrobbleResponse>(HttpMethod.Post, "collection/scrobble/", request, cancellationToken);

    public Task<CollectionStats> GetStatsAsync(CancellationToken cancellationToken = default) =>
        GetAsync<CollectionStats>("collection/stats/", cancellationToken);
}
