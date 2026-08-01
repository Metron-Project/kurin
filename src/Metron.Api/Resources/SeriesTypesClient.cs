using System.Text.Json;
using Metron.Api.Filters;
using Metron.Api.Http;
using Metron.Api.Models;

namespace Metron.Api.Resources;

public sealed class SeriesTypesClient(HttpClient http, JsonSerializerOptions jsonOptions) : ResourceClientBase(http, jsonOptions)
{
    public Task<PagedResult<SeriesType>> ListAsync(SeriesTypeFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<SeriesType>>("series_type/" + QueryStringBuilder.Build(filter), cancellationToken);

    public IAsyncEnumerable<SeriesType> ListAllAsync(SeriesTypeFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<SeriesType>("series_type/" + QueryStringBuilder.Build(filter), cancellationToken);
}
