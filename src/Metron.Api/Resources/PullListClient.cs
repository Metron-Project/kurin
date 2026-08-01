using System.Text.Json;
using Metron.Api.Filters;
using Metron.Api.Http;
using Metron.Api.Models;

namespace Metron.Api.Resources;

/// <summary>The authenticated user's pull list. Requires authentication.</summary>
public sealed class PullListClient(HttpClient http, JsonSerializerOptions jsonOptions) : ResourceClientBase(http, jsonOptions)
{
    /// <summary>Returns a single page of pull list entries.</summary>
    public Task<PagedResult<PullListRead>> ListAsync(PullListFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<PullListRead>>("pull_list/" + QueryStringBuilder.Build(filter), cancellationToken);

    /// <summary>Enumerates every pull list entry matching <paramref name="filter"/>, following pagination automatically.</summary>
    public IAsyncEnumerable<PullListRead> ListAllAsync(PullListFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<PullListRead>("pull_list/" + QueryStringBuilder.Build(filter), cancellationToken);

    /// <summary>Returns a single pull list entry by id.</summary>
    public Task<PullListRead> GetAsync(int id, CancellationToken cancellationToken = default) =>
        GetAsync<PullListRead>($"pull_list/{id}/", cancellationToken);

    /// <summary>Issues for series on the pull list. Filter by store_date_after/store_date_before.</summary>
    public Task<PagedResult<PullListIssue>> GetIssuesAsync(PullListIssuesFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<PullListIssue>>("pull_list/issues/" + QueryStringBuilder.Build(filter), cancellationToken);

    /// <summary>Enumerates every issue for series on the pull list, following pagination automatically.</summary>
    public IAsyncEnumerable<PullListIssue> GetIssuesAllAsync(PullListIssuesFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<PullListIssue>("pull_list/issues/" + QueryStringBuilder.Build(filter), cancellationToken);

    /// <summary>Returns a single page of series on the pull list.</summary>
    public Task<PagedResult<PullListSeries>> GetSeriesAsync(PullListSeriesFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<PullListSeries>>("pull_list/series/" + QueryStringBuilder.Build(filter), cancellationToken);

    /// <summary>Enumerates every series on the pull list, following pagination automatically.</summary>
    public IAsyncEnumerable<PullListSeries> GetSeriesAllAsync(PullListSeriesFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<PullListSeries>("pull_list/series/" + QueryStringBuilder.Build(filter), cancellationToken);

    /// <summary>Adds a series to the pull list.</summary>
    public Task<PullListSeries> AddSeriesAsync(int seriesId, CancellationToken cancellationToken = default) =>
        PostNoBodyAsync<PullListSeries>($"pull_list/series/add/?series_id={seriesId}", cancellationToken);

    /// <summary>Removes a series from the pull list.</summary>
    public Task RemoveSeriesAsync(int seriesPk, CancellationToken cancellationToken = default) =>
        DeleteAsync($"pull_list/series/{seriesPk}/remove/", cancellationToken);
}
