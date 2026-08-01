using System.Text.Json;
using Metron.Api.Filters;
using Metron.Api.Http;
using Metron.Api.Models;

namespace Metron.Api.Resources;

/// <summary>Teams of characters (e.g. "Avengers").</summary>
public sealed class TeamsClient(HttpClient http, JsonSerializerOptions jsonOptions) : ResourceClientBase(http, jsonOptions)
{
    /// <summary>Returns a single page of teams.</summary>
    public Task<PagedResult<TeamList>> ListAsync(TeamFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<TeamList>>("team/" + QueryStringBuilder.Build(filter), cancellationToken);

    /// <summary>Enumerates every team matching <paramref name="filter"/>, following pagination automatically.</summary>
    public IAsyncEnumerable<TeamList> ListAllAsync(TeamFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<TeamList>("team/" + QueryStringBuilder.Build(filter), cancellationToken);

    /// <summary>Returns a single team by id.</summary>
    public Task<TeamRead> GetAsync(int id, CancellationToken cancellationToken = default) =>
        GetAsync<TeamRead>($"team/{id}/", cancellationToken);

    /// <summary>Creates a new team.</summary>
    public Task<Team> CreateAsync(Team team, CancellationToken cancellationToken = default) =>
        SendFormAsync<Team>(HttpMethod.Post, "team/", team, cancellationToken);

    /// <summary>Replaces a team's data.</summary>
    public Task<Team> UpdateAsync(int id, Team team, CancellationToken cancellationToken = default) =>
        SendFormAsync<Team>(HttpMethod.Put, $"team/{id}/", team, cancellationToken);

    /// <summary>Updates a subset of a team's fields.</summary>
    public Task<Team> PartialUpdateAsync(int id, PatchedTeam team, CancellationToken cancellationToken = default) =>
        SendFormAsync<Team>(HttpMethod.Patch, $"team/{id}/", team, cancellationToken);

    /// <summary>Returns a single page of issues this team appears in.</summary>
    public Task<PagedResult<IssueList>> GetIssuesAsync(int id, TeamIssueListFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<IssueList>>($"team/{id}/issue_list/" + QueryStringBuilder.Build(filter), cancellationToken);

    /// <summary>Enumerates every issue this team appears in, following pagination automatically.</summary>
    public IAsyncEnumerable<IssueList> GetIssuesAllAsync(int id, TeamIssueListFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<IssueList>($"team/{id}/issue_list/" + QueryStringBuilder.Build(filter), cancellationToken);
}
