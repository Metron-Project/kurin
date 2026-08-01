using System.Text.Json;
using Metron.Api.Filters;
using Metron.Api.Http;
using Metron.Api.Models;

namespace Metron.Api.Resources;

/// <summary>Creator roles (e.g. "Writer", "Penciller"). Read-only.</summary>
public sealed class RolesClient(HttpClient http, JsonSerializerOptions jsonOptions) : ResourceClientBase(http, jsonOptions)
{
    /// <summary>Returns a single page of creator roles.</summary>
    public Task<PagedResult<Role>> ListAsync(RoleFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<Role>>("role/" + QueryStringBuilder.Build(filter), cancellationToken);

    /// <summary>Enumerates every creator role matching <paramref name="filter"/>, following pagination automatically.</summary>
    public IAsyncEnumerable<Role> ListAllAsync(RoleFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<Role>("role/" + QueryStringBuilder.Build(filter), cancellationToken);
}
