using System.Text.Json;
using Metron.Api.Filters;
using Metron.Api.Http;
using Metron.Api.Models;

namespace Metron.Api.Resources;

public sealed class SeriesClient(HttpClient http, JsonSerializerOptions jsonOptions) : ResourceClientBase(http, jsonOptions)
{
    public Task<PagedResult<SeriesList>> ListAsync(SeriesFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<SeriesList>>("series/" + QueryStringBuilder.Build(filter), cancellationToken);

    public IAsyncEnumerable<SeriesList> ListAllAsync(SeriesFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<SeriesList>("series/" + QueryStringBuilder.Build(filter), cancellationToken);

    public Task<SeriesRead> GetAsync(int id, CancellationToken cancellationToken = default) =>
        GetAsync<SeriesRead>($"series/{id}/", cancellationToken);

    public Task<Series> CreateAsync(Series series, CancellationToken cancellationToken = default) =>
        SendFormAsync<Series>(HttpMethod.Post, "series/", series, cancellationToken);

    public Task<Series> UpdateAsync(int id, Series series, CancellationToken cancellationToken = default) =>
        SendFormAsync<Series>(HttpMethod.Put, $"series/{id}/", series, cancellationToken);

    public Task<Series> PartialUpdateAsync(int id, PatchedSeries series, CancellationToken cancellationToken = default) =>
        SendFormAsync<Series>(HttpMethod.Patch, $"series/{id}/", series, cancellationToken);

    public Task<PagedResult<IssueList>> GetIssuesAsync(int id, SeriesIssueListFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<IssueList>>($"series/{id}/issue_list/" + QueryStringBuilder.Build(filter), cancellationToken);

    public IAsyncEnumerable<IssueList> GetIssuesAllAsync(int id, SeriesIssueListFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<IssueList>($"series/{id}/issue_list/" + QueryStringBuilder.Build(filter), cancellationToken);
}
