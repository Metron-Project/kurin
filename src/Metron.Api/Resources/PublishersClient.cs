using System.Text.Json;
using Metron.Api.Filters;
using Metron.Api.Http;
using Metron.Api.Models;

namespace Metron.Api.Resources;

public sealed class PublishersClient(HttpClient http, JsonSerializerOptions jsonOptions) : ResourceClientBase(http, jsonOptions)
{
    public Task<PagedResult<PublisherList>> ListAsync(PublisherFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<PublisherList>>("publisher/" + QueryStringBuilder.Build(filter), cancellationToken);

    public IAsyncEnumerable<PublisherList> ListAllAsync(PublisherFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<PublisherList>("publisher/" + QueryStringBuilder.Build(filter), cancellationToken);

    public Task<Publisher> GetAsync(int id, CancellationToken cancellationToken = default) =>
        GetAsync<Publisher>($"publisher/{id}/", cancellationToken);

    public Task<Publisher> CreateAsync(Publisher publisher, CancellationToken cancellationToken = default) =>
        SendFormAsync<Publisher>(HttpMethod.Post, "publisher/", publisher, cancellationToken);

    public Task<Publisher> UpdateAsync(int id, Publisher publisher, CancellationToken cancellationToken = default) =>
        SendFormAsync<Publisher>(HttpMethod.Put, $"publisher/{id}/", publisher, cancellationToken);

    public Task<Publisher> PartialUpdateAsync(int id, PatchedPublisher publisher, CancellationToken cancellationToken = default) =>
        SendFormAsync<Publisher>(HttpMethod.Patch, $"publisher/{id}/", publisher, cancellationToken);

    public Task<PagedResult<SeriesList>> GetSeriesAsync(int id, PublisherSeriesListFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<SeriesList>>($"publisher/{id}/series_list/" + QueryStringBuilder.Build(filter), cancellationToken);

    public IAsyncEnumerable<SeriesList> GetSeriesAllAsync(int id, PublisherSeriesListFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<SeriesList>($"publisher/{id}/series_list/" + QueryStringBuilder.Build(filter), cancellationToken);
}
