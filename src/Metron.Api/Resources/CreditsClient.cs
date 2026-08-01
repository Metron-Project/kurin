using System.Text.Json;
using Metron.Api.Models;

namespace Metron.Api.Resources;

public sealed class CreditsClient(HttpClient http, JsonSerializerOptions jsonOptions) : ResourceClientBase(http, jsonOptions)
{
    public Task<Credit> CreateAsync(Credit credit, CancellationToken cancellationToken = default) =>
        SendFormAsync<Credit>(HttpMethod.Post, "credit/", credit, cancellationToken);
}
