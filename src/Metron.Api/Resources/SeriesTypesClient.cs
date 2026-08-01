using System.Text.Json;
using Metron.Api.Filters;
using Metron.Api.Http;
using Metron.Api.Models;

namespace Metron.Api.Resources;

/// <summary>Series types (e.g. "Ongoing Series", "Limited Series"). Read-only.</summary>
public sealed class SeriesTypesClient(HttpClient http, JsonSerializerOptions jsonOptions) : ResourceClientBase(http, jsonOptions)
{
    /// <summary>Returns a single page of series types.</summary>
    public Task<PagedResult<SeriesType>> ListAsync(SeriesTypeFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<SeriesType>>("series_type/" + QueryStringBuilder.Build(filter), cancellationToken);

    /// <summary>Enumerates every series type matching <paramref name="filter"/>, following pagination automatically.</summary>
    public IAsyncEnumerable<SeriesType> ListAllAsync(SeriesTypeFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<SeriesType>("series_type/" + QueryStringBuilder.Build(filter), cancellationToken);
}
