using System.Text.Json;
using Metron.Api.Filters;
using Metron.Api.Http;
using Metron.Api.Models;

namespace Metron.Api.Resources;

/// <summary>Fictional universes (e.g. "Earth-616").</summary>
public sealed class UniversesClient(HttpClient http, JsonSerializerOptions jsonOptions) : ResourceClientBase(http, jsonOptions)
{
    /// <summary>Returns a single page of universes.</summary>
    public Task<PagedResult<UniverseList>> ListAsync(UniverseFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<UniverseList>>("universe/" + QueryStringBuilder.Build(filter), cancellationToken);

    /// <summary>Enumerates every universe matching <paramref name="filter"/>, following pagination automatically.</summary>
    public IAsyncEnumerable<UniverseList> ListAllAsync(UniverseFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<UniverseList>("universe/" + QueryStringBuilder.Build(filter), cancellationToken);

    /// <summary>Returns a single universe by id.</summary>
    public Task<UniverseRead> GetAsync(int id, CancellationToken cancellationToken = default) =>
        GetAsync<UniverseRead>($"universe/{id}/", cancellationToken);

    /// <summary>Creates a new universe.</summary>
    public Task<Universe> CreateAsync(Universe universe, CancellationToken cancellationToken = default) =>
        SendFormAsync<Universe>(HttpMethod.Post, "universe/", universe, cancellationToken);

    /// <summary>Replaces a universe's data.</summary>
    public Task<Universe> UpdateAsync(int id, Universe universe, CancellationToken cancellationToken = default) =>
        SendFormAsync<Universe>(HttpMethod.Put, $"universe/{id}/", universe, cancellationToken);

    /// <summary>Updates a subset of a universe's fields.</summary>
    public Task<Universe> PartialUpdateAsync(int id, PatchedUniverse universe, CancellationToken cancellationToken = default) =>
        SendFormAsync<Universe>(HttpMethod.Patch, $"universe/{id}/", universe, cancellationToken);
}
