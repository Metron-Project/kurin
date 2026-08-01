using System.Text.Json;
using Metron.Api.Models;

namespace Metron.Api.Resources;

/// <summary>
/// Credits linking a creator to an issue with a role (e.g. "penciller"). The API only exposes
/// create for this resource; credits are otherwise read as part of an issue's data.
/// </summary>
public sealed class CreditsClient(HttpClient http, JsonSerializerOptions jsonOptions) : ResourceClientBase(http, jsonOptions)
{
    /// <summary>Creates a new credit.</summary>
    public Task<Credit> CreateAsync(Credit credit, CancellationToken cancellationToken = default) =>
        SendFormAsync<Credit>(HttpMethod.Post, "credit/", credit, cancellationToken);
}
