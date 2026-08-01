using System.Text.Json;
using Metron.Api.Filters;
using Metron.Api.Http;
using Metron.Api.Models;

namespace Metron.Api.Resources;

public sealed class UniversesClient(HttpClient http, JsonSerializerOptions jsonOptions) : ResourceClientBase(http, jsonOptions)
{
    public Task<PagedResult<UniverseList>> ListAsync(UniverseFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<UniverseList>>("universe/" + QueryStringBuilder.Build(filter), cancellationToken);

    public IAsyncEnumerable<UniverseList> ListAllAsync(UniverseFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<UniverseList>("universe/" + QueryStringBuilder.Build(filter), cancellationToken);

    public Task<UniverseRead> GetAsync(int id, CancellationToken cancellationToken = default) =>
        GetAsync<UniverseRead>($"universe/{id}/", cancellationToken);

    public Task<Universe> CreateAsync(Universe universe, CancellationToken cancellationToken = default) =>
        SendFormAsync<Universe>(HttpMethod.Post, "universe/", universe, cancellationToken);

    public Task<Universe> UpdateAsync(int id, Universe universe, CancellationToken cancellationToken = default) =>
        SendFormAsync<Universe>(HttpMethod.Put, $"universe/{id}/", universe, cancellationToken);

    public Task<Universe> PartialUpdateAsync(int id, PatchedUniverse universe, CancellationToken cancellationToken = default) =>
        SendFormAsync<Universe>(HttpMethod.Patch, $"universe/{id}/", universe, cancellationToken);
}
