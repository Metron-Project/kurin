using System.Text.Json;
using Metron.Api.Filters;
using Metron.Api.Http;
using Metron.Api.Models;

namespace Metron.Api.Resources;

public sealed class IssuesClient(HttpClient http, JsonSerializerOptions jsonOptions) : ResourceClientBase(http, jsonOptions)
{
    public Task<PagedResult<IssueList>> ListAsync(IssueFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<IssueList>>("issue/" + QueryStringBuilder.Build(filter), cancellationToken);

    public IAsyncEnumerable<IssueList> ListAllAsync(IssueFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<IssueList>("issue/" + QueryStringBuilder.Build(filter), cancellationToken);

    public Task<IssueRead> GetAsync(int id, CancellationToken cancellationToken = default) =>
        GetAsync<IssueRead>($"issue/{id}/", cancellationToken);

    public Task<Issue> CreateAsync(Issue issue, CancellationToken cancellationToken = default) =>
        SendFormAsync<Issue>(HttpMethod.Post, "issue/", issue, cancellationToken);

    public Task<Issue> UpdateAsync(int id, Issue issue, CancellationToken cancellationToken = default) =>
        SendFormAsync<Issue>(HttpMethod.Put, $"issue/{id}/", issue, cancellationToken);

    public Task<Issue> PartialUpdateAsync(int id, PatchedIssue issue, CancellationToken cancellationToken = default) =>
        SendFormAsync<Issue>(HttpMethod.Patch, $"issue/{id}/", issue, cancellationToken);
}
