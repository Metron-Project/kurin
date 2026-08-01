using System.Text.Json;
using Metron.Api.Filters;
using Metron.Api.Http;
using Metron.Api.Models;

namespace Metron.Api.Resources;

public sealed class RolesClient(HttpClient http, JsonSerializerOptions jsonOptions) : ResourceClientBase(http, jsonOptions)
{
    public Task<PagedResult<Role>> ListAsync(RoleFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<Role>>("role/" + QueryStringBuilder.Build(filter), cancellationToken);

    public IAsyncEnumerable<Role> ListAllAsync(RoleFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<Role>("role/" + QueryStringBuilder.Build(filter), cancellationToken);
}
