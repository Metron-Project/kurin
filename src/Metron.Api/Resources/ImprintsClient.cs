using System.Text.Json;
using Metron.Api.Filters;
using Metron.Api.Http;
using Metron.Api.Models;

namespace Metron.Api.Resources;

public sealed class ImprintsClient(HttpClient http, JsonSerializerOptions jsonOptions) : ResourceClientBase(http, jsonOptions)
{
    public Task<PagedResult<ImprintList>> ListAsync(ImprintFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<ImprintList>>("imprint/" + QueryStringBuilder.Build(filter), cancellationToken);

    public IAsyncEnumerable<ImprintList> ListAllAsync(ImprintFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<ImprintList>("imprint/" + QueryStringBuilder.Build(filter), cancellationToken);

    public Task<ImprintRead> GetAsync(int id, CancellationToken cancellationToken = default) =>
        GetAsync<ImprintRead>($"imprint/{id}/", cancellationToken);

    public Task<Imprint> CreateAsync(Imprint imprint, CancellationToken cancellationToken = default) =>
        SendFormAsync<Imprint>(HttpMethod.Post, "imprint/", imprint, cancellationToken);

    public Task<Imprint> UpdateAsync(int id, Imprint imprint, CancellationToken cancellationToken = default) =>
        SendFormAsync<Imprint>(HttpMethod.Put, $"imprint/{id}/", imprint, cancellationToken);

    public Task<Imprint> PartialUpdateAsync(int id, PatchedImprint imprint, CancellationToken cancellationToken = default) =>
        SendFormAsync<Imprint>(HttpMethod.Patch, $"imprint/{id}/", imprint, cancellationToken);
}
