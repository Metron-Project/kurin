using System.Text.Json;
using Metron.Api.Filters;
using Metron.Api.Http;
using Metron.Api.Models;

namespace Metron.Api.Resources;

public sealed class ArcsClient(HttpClient http, JsonSerializerOptions jsonOptions) : ResourceClientBase(http, jsonOptions)
{
    public Task<PagedResult<ArcList>> ListAsync(ArcFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<ArcList>>("arc/" + QueryStringBuilder.Build(filter), cancellationToken);

    public IAsyncEnumerable<ArcList> ListAllAsync(ArcFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<ArcList>("arc/" + QueryStringBuilder.Build(filter), cancellationToken);

    public Task<Arc> GetAsync(int id, CancellationToken cancellationToken = default) =>
        GetAsync<Arc>($"arc/{id}/", cancellationToken);

    public Task<Arc> CreateAsync(Arc arc, CancellationToken cancellationToken = default) =>
        SendFormAsync<Arc>(HttpMethod.Post, "arc/", arc, cancellationToken);

    public Task<Arc> UpdateAsync(int id, Arc arc, CancellationToken cancellationToken = default) =>
        SendFormAsync<Arc>(HttpMethod.Put, $"arc/{id}/", arc, cancellationToken);

    public Task<Arc> PartialUpdateAsync(int id, PatchedArc arc, CancellationToken cancellationToken = default) =>
        SendFormAsync<Arc>(HttpMethod.Patch, $"arc/{id}/", arc, cancellationToken);

    public Task<PagedResult<IssueList>> GetIssuesAsync(int id, ArcIssueListFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<IssueList>>($"arc/{id}/issue_list/" + QueryStringBuilder.Build(filter), cancellationToken);

    public IAsyncEnumerable<IssueList> GetIssuesAllAsync(int id, ArcIssueListFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<IssueList>($"arc/{id}/issue_list/" + QueryStringBuilder.Build(filter), cancellationToken);
}
