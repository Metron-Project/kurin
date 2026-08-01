using System.Text.Json;
using Metron.Api.Filters;
using Metron.Api.Http;
using Metron.Api.Models;

namespace Metron.Api.Resources;

/// <summary>Comic creators (writers, artists, etc.).</summary>
public sealed class CreatorsClient(HttpClient http, JsonSerializerOptions jsonOptions) : ResourceClientBase(http, jsonOptions)
{
    /// <summary>Returns a single page of creators.</summary>
    public Task<PagedResult<CreatorList>> ListAsync(CreatorFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<CreatorList>>("creator/" + QueryStringBuilder.Build(filter), cancellationToken);

    /// <summary>Enumerates every creator matching <paramref name="filter"/>, following pagination automatically.</summary>
    public IAsyncEnumerable<CreatorList> ListAllAsync(CreatorFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<CreatorList>("creator/" + QueryStringBuilder.Build(filter), cancellationToken);

    /// <summary>Returns a single creator by id.</summary>
    public Task<Creator> GetAsync(int id, CancellationToken cancellationToken = default) =>
        GetAsync<Creator>($"creator/{id}/", cancellationToken);

    /// <summary>Creates a new creator.</summary>
    public Task<Creator> CreateAsync(Creator creator, CancellationToken cancellationToken = default) =>
        SendFormAsync<Creator>(HttpMethod.Post, "creator/", creator, cancellationToken);

    /// <summary>Replaces a creator's data.</summary>
    public Task<Creator> UpdateAsync(int id, Creator creator, CancellationToken cancellationToken = default) =>
        SendFormAsync<Creator>(HttpMethod.Put, $"creator/{id}/", creator, cancellationToken);

    /// <summary>Updates a subset of a creator's fields.</summary>
    public Task<Creator> PartialUpdateAsync(int id, PatchedCreator creator, CancellationToken cancellationToken = default) =>
        SendFormAsync<Creator>(HttpMethod.Patch, $"creator/{id}/", creator, cancellationToken);
}
