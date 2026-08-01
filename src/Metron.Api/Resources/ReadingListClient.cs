using System.Text.Json;
using Metron.Api.Filters;
using Metron.Api.Http;
using Metron.Api.Models;

namespace Metron.Api.Resources;

/// <summary>Read-only: reading lists are managed outside the API.</summary>
public sealed class ReadingListClient(HttpClient http, JsonSerializerOptions jsonOptions) : ResourceClientBase(http, jsonOptions)
{
    /// <summary>Returns a single page of reading lists visible to the authenticated user.</summary>
    public Task<PagedResult<ReadingListList>> ListAsync(ReadingListFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<ReadingListList>>("reading_list/" + QueryStringBuilder.Build(filter), cancellationToken);

    /// <summary>Enumerates every reading list matching <paramref name="filter"/>, following pagination automatically.</summary>
    public IAsyncEnumerable<ReadingListList> ListAllAsync(ReadingListFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<ReadingListList>("reading_list/" + QueryStringBuilder.Build(filter), cancellationToken);

    /// <summary>Returns a single reading list by id.</summary>
    public Task<ReadingListRead> GetAsync(int id, CancellationToken cancellationToken = default) =>
        GetAsync<ReadingListRead>($"reading_list/{id}/", cancellationToken);

    /// <summary>Returns a single page of items in this reading list.</summary>
    public Task<PagedResult<ReadingListItem>> GetItemsAsync(int id, ReadingListItemsFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<ReadingListItem>>($"reading_list/{id}/items/" + QueryStringBuilder.Build(filter), cancellationToken);

    /// <summary>Enumerates every item in this reading list, following pagination automatically.</summary>
    public IAsyncEnumerable<ReadingListItem> GetItemsAllAsync(int id, ReadingListItemsFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<ReadingListItem>($"reading_list/{id}/items/" + QueryStringBuilder.Build(filter), cancellationToken);
}
