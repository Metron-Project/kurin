using System.Text.Json;
using Metron.Api.Filters;
using Metron.Api.Http;
using Metron.Api.Models;

namespace Metron.Api.Resources;

/// <summary>The authenticated user's pull list. Requires authentication.</summary>
public sealed class PullListClient(HttpClient http, JsonSerializerOptions jsonOptions) : ResourceClientBase(http, jsonOptions)
{
    public Task<PagedResult<PullListRead>> ListAsync(PullListFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<PullListRead>>("pull_list/" + QueryStringBuilder.Build(filter), cancellationToken);

    public IAsyncEnumerable<PullListRead> ListAllAsync(PullListFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<PullListRead>("pull_list/" + QueryStringBuilder.Build(filter), cancellationToken);

    public Task<PullListRead> GetAsync(int id, CancellationToken cancellationToken = default) =>
        GetAsync<PullListRead>($"pull_list/{id}/", cancellationToken);

    /// <summary>Issues for series on the pull list. Filter by store_date_after/store_date_before.</summary>
    public Task<PagedResult<PullListIssue>> GetIssuesAsync(PullListIssuesFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<PullListIssue>>("pull_list/issues/" + QueryStringBuilder.Build(filter), cancellationToken);

    public IAsyncEnumerable<PullListIssue> GetIssuesAllAsync(PullListIssuesFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<PullListIssue>("pull_list/issues/" + QueryStringBuilder.Build(filter), cancellationToken);

    public Task<PagedResult<PullListSeries>> GetSeriesAsync(PullListSeriesFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<PullListSeries>>("pull_list/series/" + QueryStringBuilder.Build(filter), cancellationToken);

    public IAsyncEnumerable<PullListSeries> GetSeriesAllAsync(PullListSeriesFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<PullListSeries>("pull_list/series/" + QueryStringBuilder.Build(filter), cancellationToken);

    public Task<PullListSeries> AddSeriesAsync(int seriesId, CancellationToken cancellationToken = default) =>
        PostNoBodyAsync<PullListSeries>($"pull_list/series/add/?series_id={seriesId}", cancellationToken);

    public Task RemoveSeriesAsync(int seriesPk, CancellationToken cancellationToken = default) =>
        DeleteAsync($"pull_list/series/{seriesPk}/remove/", cancellationToken);
}
