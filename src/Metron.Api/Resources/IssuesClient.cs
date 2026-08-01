using System.Text.Json;
using Metron.Api.Filters;
using Metron.Api.Http;
using Metron.Api.Models;

namespace Metron.Api.Resources;

/// <summary>Individual comic issues.</summary>
public sealed class IssuesClient(HttpClient http, JsonSerializerOptions jsonOptions) : ResourceClientBase(http, jsonOptions)
{
    /// <summary>Returns a single page of issues.</summary>
    public Task<PagedResult<IssueList>> ListAsync(IssueFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<IssueList>>("issue/" + QueryStringBuilder.Build(filter), cancellationToken);

    /// <summary>Enumerates every issue matching <paramref name="filter"/>, following pagination automatically.</summary>
    public IAsyncEnumerable<IssueList> ListAllAsync(IssueFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<IssueList>("issue/" + QueryStringBuilder.Build(filter), cancellationToken);

    /// <summary>Returns a single issue by id, with its series/arcs/characters/teams expanded.</summary>
    public Task<IssueRead> GetAsync(int id, CancellationToken cancellationToken = default) =>
        GetAsync<IssueRead>($"issue/{id}/", cancellationToken);

    /// <summary>Creates a new issue.</summary>
    public Task<Issue> CreateAsync(Issue issue, CancellationToken cancellationToken = default) =>
        SendFormAsync<Issue>(HttpMethod.Post, "issue/", issue, cancellationToken);

    /// <summary>Replaces an issue's data.</summary>
    public Task<Issue> UpdateAsync(int id, Issue issue, CancellationToken cancellationToken = default) =>
        SendFormAsync<Issue>(HttpMethod.Put, $"issue/{id}/", issue, cancellationToken);

    /// <summary>Updates a subset of an issue's fields.</summary>
    public Task<Issue> PartialUpdateAsync(int id, PatchedIssue issue, CancellationToken cancellationToken = default) =>
        SendFormAsync<Issue>(HttpMethod.Patch, $"issue/{id}/", issue, cancellationToken);
}
