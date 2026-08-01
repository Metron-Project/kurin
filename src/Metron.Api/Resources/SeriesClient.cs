using System.Text.Json;
using Metron.Api.Filters;
using Metron.Api.Http;
using Metron.Api.Models;

namespace Metron.Api.Resources;

/// <summary>Comic series (e.g. "The Amazing Spider-Man").</summary>
public sealed class SeriesClient(HttpClient http, JsonSerializerOptions jsonOptions) : ResourceClientBase(http, jsonOptions)
{
    /// <summary>Returns a single page of series.</summary>
    public Task<PagedResult<SeriesList>> ListAsync(SeriesFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<SeriesList>>("series/" + QueryStringBuilder.Build(filter), cancellationToken);

    /// <summary>Enumerates every series matching <paramref name="filter"/>, following pagination automatically.</summary>
    public IAsyncEnumerable<SeriesList> ListAllAsync(SeriesFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<SeriesList>("series/" + QueryStringBuilder.Build(filter), cancellationToken);

    /// <summary>Returns a single series by id, with its publisher/imprint/series type expanded.</summary>
    public Task<SeriesRead> GetAsync(int id, CancellationToken cancellationToken = default) =>
        GetAsync<SeriesRead>($"series/{id}/", cancellationToken);

    /// <summary>Creates a new series.</summary>
    public Task<Series> CreateAsync(Series series, CancellationToken cancellationToken = default) =>
        SendFormAsync<Series>(HttpMethod.Post, "series/", series, cancellationToken);

    /// <summary>Replaces a series' data.</summary>
    public Task<Series> UpdateAsync(int id, Series series, CancellationToken cancellationToken = default) =>
        SendFormAsync<Series>(HttpMethod.Put, $"series/{id}/", series, cancellationToken);

    /// <summary>Updates a subset of a series' fields.</summary>
    public Task<Series> PartialUpdateAsync(int id, PatchedSeries series, CancellationToken cancellationToken = default) =>
        SendFormAsync<Series>(HttpMethod.Patch, $"series/{id}/", series, cancellationToken);

    /// <summary>Returns a single page of issues in this series.</summary>
    public Task<PagedResult<IssueList>> GetIssuesAsync(int id, SeriesIssueListFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<IssueList>>($"series/{id}/issue_list/" + QueryStringBuilder.Build(filter), cancellationToken);

    /// <summary>Enumerates every issue in this series, following pagination automatically.</summary>
    public IAsyncEnumerable<IssueList> GetIssuesAllAsync(int id, SeriesIssueListFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<IssueList>($"series/{id}/issue_list/" + QueryStringBuilder.Build(filter), cancellationToken);
}
