using System.Text.Json;
using Metron.Api.Filters;
using Metron.Api.Http;
using Metron.Api.Models;

namespace Metron.Api.Resources;

/// <summary>Publisher imprints (e.g. "Vertigo").</summary>
public sealed class ImprintsClient(HttpClient http, JsonSerializerOptions jsonOptions) : ResourceClientBase(http, jsonOptions)
{
    /// <summary>Returns a single page of imprints.</summary>
    public Task<PagedResult<ImprintList>> ListAsync(ImprintFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<ImprintList>>("imprint/" + QueryStringBuilder.Build(filter), cancellationToken);

    /// <summary>Enumerates every imprint matching <paramref name="filter"/>, following pagination automatically.</summary>
    public IAsyncEnumerable<ImprintList> ListAllAsync(ImprintFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<ImprintList>("imprint/" + QueryStringBuilder.Build(filter), cancellationToken);

    /// <summary>Returns a single imprint by id, with its publisher expanded.</summary>
    public Task<ImprintRead> GetAsync(int id, CancellationToken cancellationToken = default) =>
        GetAsync<ImprintRead>($"imprint/{id}/", cancellationToken);

    /// <summary>Creates a new imprint.</summary>
    public Task<Imprint> CreateAsync(Imprint imprint, CancellationToken cancellationToken = default) =>
        SendFormAsync<Imprint>(HttpMethod.Post, "imprint/", imprint, cancellationToken);

    /// <summary>Replaces an imprint's data.</summary>
    public Task<Imprint> UpdateAsync(int id, Imprint imprint, CancellationToken cancellationToken = default) =>
        SendFormAsync<Imprint>(HttpMethod.Put, $"imprint/{id}/", imprint, cancellationToken);

    /// <summary>Updates a subset of an imprint's fields.</summary>
    public Task<Imprint> PartialUpdateAsync(int id, PatchedImprint imprint, CancellationToken cancellationToken = default) =>
        SendFormAsync<Imprint>(HttpMethod.Patch, $"imprint/{id}/", imprint, cancellationToken);
}
