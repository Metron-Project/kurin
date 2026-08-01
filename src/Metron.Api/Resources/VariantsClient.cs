using System.Text.Json;
using Metron.Api.Models;

namespace Metron.Api.Resources;

/// <summary>
/// The API only exposes create/update/partial_update for variant covers (no list/retrieve);
/// they're read via the parent Issue's data instead.
/// </summary>
public sealed class VariantsClient(HttpClient http, JsonSerializerOptions jsonOptions) : ResourceClientBase(http, jsonOptions)
{
    public Task<Variant> CreateAsync(Variant variant, CancellationToken cancellationToken = default) =>
        SendFormAsync<Variant>(HttpMethod.Post, "variant/", variant, cancellationToken);

    public Task<Variant> UpdateAsync(int id, Variant variant, CancellationToken cancellationToken = default) =>
        SendFormAsync<Variant>(HttpMethod.Put, $"variant/{id}/", variant, cancellationToken);

    public Task<Variant> PartialUpdateAsync(int id, PatchedVariant variant, CancellationToken cancellationToken = default) =>
        SendFormAsync<Variant>(HttpMethod.Patch, $"variant/{id}/", variant, cancellationToken);
}
