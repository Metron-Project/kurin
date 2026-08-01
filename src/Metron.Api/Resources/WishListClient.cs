using System.Text.Json;
using Metron.Api.Filters;
using Metron.Api.Http;
using Metron.Api.Models;

namespace Metron.Api.Resources;

/// <summary>The authenticated user's wish list. Requires authentication.</summary>
public sealed class WishListClient(HttpClient http, JsonSerializerOptions jsonOptions) : ResourceClientBase(http, jsonOptions)
{
    public Task<PagedResult<WishList>> ListAsync(WishListFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<WishList>>("wish_list/" + QueryStringBuilder.Build(filter), cancellationToken);

    public IAsyncEnumerable<WishList> ListAllAsync(WishListFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<WishList>("wish_list/" + QueryStringBuilder.Build(filter), cancellationToken);

    public Task<WishList> GetAsync(int id, CancellationToken cancellationToken = default) =>
        GetAsync<WishList>($"wish_list/{id}/", cancellationToken);

    public Task<PagedResult<WishListItemList>> GetItemsAsync(WishListItemsFilter? filter = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<WishListItemList>>("wish_list/items/" + QueryStringBuilder.Build(filter), cancellationToken);

    public IAsyncEnumerable<WishListItemList> GetItemsAllAsync(WishListItemsFilter? filter = null, CancellationToken cancellationToken = default) =>
        AutoPageAsync<WishListItemList>("wish_list/items/" + QueryStringBuilder.Build(filter), cancellationToken);

    /// <summary>Adds an issue to the wish list.</summary>
    public Task<WishListItemRead> AddItemAsync(WishListAddItem item, CancellationToken cancellationToken = default) =>
        SendFormAsync<WishListItemRead>(HttpMethod.Post, "wish_list/items/add/", item, cancellationToken);

    /// <summary>Marks a wish list item as acquired and creates a collection item for it.</summary>
    public Task AcquireItemAsync(int itemPk, AcquireWishListItem acquisition, CancellationToken cancellationToken = default) =>
        SendFormNoResultAsync(HttpMethod.Post, $"wish_list/items/{itemPk}/acquire/", acquisition, cancellationToken);

    public Task RemoveItemAsync(int itemPk, CancellationToken cancellationToken = default) =>
        DeleteAsync($"wish_list/items/{itemPk}/remove/", cancellationToken);
}
