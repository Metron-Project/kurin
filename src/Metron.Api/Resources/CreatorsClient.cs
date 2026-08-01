using System.Text.Json;
using Metron.Api.Filters;
using Metron.Api.Http;
using Metron.Api.Models;

namespace Metron.Api.Resources;

public sealed class CreatorsClient(HttpClient http, JsonSerializerOptions jsonOptions) : ResourceClientBase(http, jsonOptions)
{
    public Task<PagedResult<CreatorList>> ListAsync(CreatorFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<CreatorList>>("creator/" + QueryStringBuilder.Build(filter), cancellationToken);

    public IAsyncEnumerable<CreatorList> ListAllAsync(CreatorFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<CreatorList>("creator/" + QueryStringBuilder.Build(filter), cancellationToken);

    public Task<Creator> GetAsync(int id, CancellationToken cancellationToken = default) =>
        GetAsync<Creator>($"creator/{id}/", cancellationToken);

    public Task<Creator> CreateAsync(Creator creator, CancellationToken cancellationToken = default) =>
        SendFormAsync<Creator>(HttpMethod.Post, "creator/", creator, cancellationToken);

    public Task<Creator> UpdateAsync(int id, Creator creator, CancellationToken cancellationToken = default) =>
        SendFormAsync<Creator>(HttpMethod.Put, $"creator/{id}/", creator, cancellationToken);

    public Task<Creator> PartialUpdateAsync(int id, PatchedCreator creator, CancellationToken cancellationToken = default) =>
        SendFormAsync<Creator>(HttpMethod.Patch, $"creator/{id}/", creator, cancellationToken);
}
