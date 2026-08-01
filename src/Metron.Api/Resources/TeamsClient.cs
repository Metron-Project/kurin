using System.Text.Json;
using Metron.Api.Filters;
using Metron.Api.Http;
using Metron.Api.Models;

namespace Metron.Api.Resources;

public sealed class TeamsClient(HttpClient http, JsonSerializerOptions jsonOptions) : ResourceClientBase(http, jsonOptions)
{
    public Task<PagedResult<TeamList>> ListAsync(TeamFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<TeamList>>("team/" + QueryStringBuilder.Build(filter), cancellationToken);

    public IAsyncEnumerable<TeamList> ListAllAsync(TeamFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<TeamList>("team/" + QueryStringBuilder.Build(filter), cancellationToken);

    public Task<TeamRead> GetAsync(int id, CancellationToken cancellationToken = default) =>
        GetAsync<TeamRead>($"team/{id}/", cancellationToken);

    public Task<Team> CreateAsync(Team team, CancellationToken cancellationToken = default) =>
        SendFormAsync<Team>(HttpMethod.Post, "team/", team, cancellationToken);

    public Task<Team> UpdateAsync(int id, Team team, CancellationToken cancellationToken = default) =>
        SendFormAsync<Team>(HttpMethod.Put, $"team/{id}/", team, cancellationToken);

    public Task<Team> PartialUpdateAsync(int id, PatchedTeam team, CancellationToken cancellationToken = default) =>
        SendFormAsync<Team>(HttpMethod.Patch, $"team/{id}/", team, cancellationToken);

    public Task<PagedResult<IssueList>> GetIssuesAsync(int id, TeamIssueListFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<IssueList>>($"team/{id}/issue_list/" + QueryStringBuilder.Build(filter), cancellationToken);

    public IAsyncEnumerable<IssueList> GetIssuesAllAsync(int id, TeamIssueListFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<IssueList>($"team/{id}/issue_list/" + QueryStringBuilder.Build(filter), cancellationToken);
}
