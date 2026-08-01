using System.Text.Json;
using Metron.Api.Filters;
using Metron.Api.Http;
using Metron.Api.Models;

namespace Metron.Api.Resources;

/// <summary>Story arcs (e.g. "Infinity Gauntlet").</summary>
public sealed class ArcsClient(HttpClient http, JsonSerializerOptions jsonOptions) : ResourceClientBase(http, jsonOptions)
{
    /// <summary>Returns a single page of arcs.</summary>
    public Task<PagedResult<ArcList>> ListAsync(ArcFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<ArcList>>("arc/" + QueryStringBuilder.Build(filter), cancellationToken);

    /// <summary>Enumerates every arc matching <paramref name="filter"/>, following pagination automatically.</summary>
    public IAsyncEnumerable<ArcList> ListAllAsync(ArcFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<ArcList>("arc/" + QueryStringBuilder.Build(filter), cancellationToken);

    /// <summary>Returns a single arc by id.</summary>
    public Task<Arc> GetAsync(int id, CancellationToken cancellationToken = default) =>
        GetAsync<Arc>($"arc/{id}/", cancellationToken);

    /// <summary>Creates a new arc.</summary>
    public Task<Arc> CreateAsync(Arc arc, CancellationToken cancellationToken = default) =>
        SendFormAsync<Arc>(HttpMethod.Post, "arc/", arc, cancellationToken);

    /// <summary>Replaces an arc's data.</summary>
    public Task<Arc> UpdateAsync(int id, Arc arc, CancellationToken cancellationToken = default) =>
        SendFormAsync<Arc>(HttpMethod.Put, $"arc/{id}/", arc, cancellationToken);

    /// <summary>Updates a subset of an arc's fields.</summary>
    public Task<Arc> PartialUpdateAsync(int id, PatchedArc arc, CancellationToken cancellationToken = default) =>
        SendFormAsync<Arc>(HttpMethod.Patch, $"arc/{id}/", arc, cancellationToken);

    /// <summary>Returns a single page of issues that belong to this arc.</summary>
    public Task<PagedResult<IssueList>> GetIssuesAsync(int id, ArcIssueListFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<IssueList>>($"arc/{id}/issue_list/" + QueryStringBuilder.Build(filter), cancellationToken);

    /// <summary>Enumerates every issue that belongs to this arc, following pagination automatically.</summary>
    public IAsyncEnumerable<IssueList> GetIssuesAllAsync(int id, ArcIssueListFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<IssueList>($"arc/{id}/issue_list/" + QueryStringBuilder.Build(filter), cancellationToken);
}
