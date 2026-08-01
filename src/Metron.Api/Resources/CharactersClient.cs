using System.Text.Json;
using Metron.Api.Filters;
using Metron.Api.Http;
using Metron.Api.Models;

namespace Metron.Api.Resources;

public sealed class CharactersClient(HttpClient http, JsonSerializerOptions jsonOptions) : ResourceClientBase(http, jsonOptions)
{
    public Task<PagedResult<CharacterList>> ListAsync(CharacterFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<CharacterList>>("character/" + QueryStringBuilder.Build(filter), cancellationToken);

    public IAsyncEnumerable<CharacterList> ListAllAsync(CharacterFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<CharacterList>("character/" + QueryStringBuilder.Build(filter), cancellationToken);

    public Task<CharacterRead> GetAsync(int id, CancellationToken cancellationToken = default) =>
        GetAsync<CharacterRead>($"character/{id}/", cancellationToken);

    public Task<Character> CreateAsync(Character character, CancellationToken cancellationToken = default) =>
        SendFormAsync<Character>(HttpMethod.Post, "character/", character, cancellationToken);

    public Task<Character> UpdateAsync(int id, Character character, CancellationToken cancellationToken = default) =>
        SendFormAsync<Character>(HttpMethod.Put, $"character/{id}/", character, cancellationToken);

    public Task<Character> PartialUpdateAsync(int id, PatchedCharacter character, CancellationToken cancellationToken = default) =>
        SendFormAsync<Character>(HttpMethod.Patch, $"character/{id}/", character, cancellationToken);

    public Task<PagedResult<IssueList>> GetIssuesAsync(int id, CharacterIssueListFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<IssueList>>($"character/{id}/issue_list/" + QueryStringBuilder.Build(filter), cancellationToken);

    public IAsyncEnumerable<IssueList> GetIssuesAllAsync(int id, CharacterIssueListFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<IssueList>($"character/{id}/issue_list/" + QueryStringBuilder.Build(filter), cancellationToken);
}
