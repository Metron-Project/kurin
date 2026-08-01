using System.Text.Json;
using Metron.Api.Filters;
using Metron.Api.Http;
using Metron.Api.Models;

namespace Metron.Api.Resources;

/// <summary>Comic characters (e.g. "Spider-Man").</summary>
public sealed class CharactersClient(HttpClient http, JsonSerializerOptions jsonOptions) : ResourceClientBase(http, jsonOptions)
{
    /// <summary>Returns a single page of characters.</summary>
    public Task<PagedResult<CharacterList>> ListAsync(CharacterFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<CharacterList>>("character/" + QueryStringBuilder.Build(filter), cancellationToken);

    /// <summary>Enumerates every character matching <paramref name="filter"/>, following pagination automatically.</summary>
    public IAsyncEnumerable<CharacterList> ListAllAsync(CharacterFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<CharacterList>("character/" + QueryStringBuilder.Build(filter), cancellationToken);

    /// <summary>Returns a single character by id, with its creators/teams/universes expanded.</summary>
    public Task<CharacterRead> GetAsync(int id, CancellationToken cancellationToken = default) =>
        GetAsync<CharacterRead>($"character/{id}/", cancellationToken);

    /// <summary>Creates a new character.</summary>
    public Task<Character> CreateAsync(Character character, CancellationToken cancellationToken = default) =>
        SendFormAsync<Character>(HttpMethod.Post, "character/", character, cancellationToken);

    /// <summary>Replaces a character's data.</summary>
    public Task<Character> UpdateAsync(int id, Character character, CancellationToken cancellationToken = default) =>
        SendFormAsync<Character>(HttpMethod.Put, $"character/{id}/", character, cancellationToken);

    /// <summary>Updates a subset of a character's fields.</summary>
    public Task<Character> PartialUpdateAsync(int id, PatchedCharacter character, CancellationToken cancellationToken = default) =>
        SendFormAsync<Character>(HttpMethod.Patch, $"character/{id}/", character, cancellationToken);

    /// <summary>Returns a single page of issues this character appears in.</summary>
    public Task<PagedResult<IssueList>> GetIssuesAsync(int id, CharacterIssueListFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<IssueList>>($"character/{id}/issue_list/" + QueryStringBuilder.Build(filter), cancellationToken);

    /// <summary>Enumerates every issue this character appears in, following pagination automatically.</summary>
    public IAsyncEnumerable<IssueList> GetIssuesAllAsync(int id, CharacterIssueListFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<IssueList>($"character/{id}/issue_list/" + QueryStringBuilder.Build(filter), cancellationToken);
}
