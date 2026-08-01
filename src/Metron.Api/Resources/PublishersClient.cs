using System.Text.Json;
using Metron.Api.Filters;
using Metron.Api.Http;
using Metron.Api.Models;

namespace Metron.Api.Resources;

/// <summary>Comic publishers (e.g. "Marvel").</summary>
public sealed class PublishersClient(HttpClient http, JsonSerializerOptions jsonOptions) : ResourceClientBase(http, jsonOptions)
{
    /// <summary>Returns a single page of publishers.</summary>
    public Task<PagedResult<PublisherList>> ListAsync(PublisherFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<PublisherList>>("publisher/" + QueryStringBuilder.Build(filter), cancellationToken);

    /// <summary>Enumerates every publisher matching <paramref name="filter"/>, following pagination automatically.</summary>
    public IAsyncEnumerable<PublisherList> ListAllAsync(PublisherFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<PublisherList>("publisher/" + QueryStringBuilder.Build(filter), cancellationToken);

    /// <summary>Returns a single publisher by id.</summary>
    public Task<Publisher> GetAsync(int id, CancellationToken cancellationToken = default) =>
        GetAsync<Publisher>($"publisher/{id}/", cancellationToken);

    /// <summary>Creates a new publisher.</summary>
    public Task<Publisher> CreateAsync(Publisher publisher, CancellationToken cancellationToken = default) =>
        SendFormAsync<Publisher>(HttpMethod.Post, "publisher/", publisher, cancellationToken);

    /// <summary>Replaces a publisher's data.</summary>
    public Task<Publisher> UpdateAsync(int id, Publisher publisher, CancellationToken cancellationToken = default) =>
        SendFormAsync<Publisher>(HttpMethod.Put, $"publisher/{id}/", publisher, cancellationToken);

    /// <summary>Updates a subset of a publisher's fields.</summary>
    public Task<Publisher> PartialUpdateAsync(int id, PatchedPublisher publisher, CancellationToken cancellationToken = default) =>
        SendFormAsync<Publisher>(HttpMethod.Patch, $"publisher/{id}/", publisher, cancellationToken);

    /// <summary>Returns a single page of series published by this publisher.</summary>
    public Task<PagedResult<SeriesList>> GetSeriesAsync(int id, PublisherSeriesListFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<SeriesList>>($"publisher/{id}/series_list/" + QueryStringBuilder.Build(filter), cancellationToken);

    /// <summary>Enumerates every series published by this publisher, following pagination automatically.</summary>
    public IAsyncEnumerable<SeriesList> GetSeriesAllAsync(int id, PublisherSeriesListFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<SeriesList>($"publisher/{id}/series_list/" + QueryStringBuilder.Build(filter), cancellationToken);
}
