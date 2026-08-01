using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Metron.Api.Exceptions;
using Metron.Api.Http;
using Metron.Api.Models;

namespace Metron.Api.Resources;

/// <summary>Shared HTTP plumbing for all resource clients (Arc, Character, Issue, ...).</summary>
public abstract class ResourceClientBase
{
    private protected readonly HttpClient Http;
    private protected readonly JsonSerializerOptions JsonOptions;

    private protected ResourceClientBase(HttpClient http, JsonSerializerOptions jsonOptions)
    {
        Http = http;
        JsonOptions = jsonOptions;
    }

    private protected async Task<T> GetAsync<T>(string requestUri, CancellationToken cancellationToken)
    {
        using var response = await Http.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response, cancellationToken).ConfigureAwait(false);
        return (await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken).ConfigureAwait(false))!;
    }

    /// <summary>Follows the `next` link of a paged response until exhausted.</summary>
    private protected async IAsyncEnumerable<T> AutoPageAsync<T>(
        string firstRequestUri, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        string? next = firstRequestUri;
        while (next is not null)
        {
            var page = await GetAsync<PagedResult<T>>(next, cancellationToken).ConfigureAwait(false);
            foreach (var item in page.Results)
            {
                yield return item;
            }

            next = page.Next;
        }
    }

    private protected async Task<T> SendFormAsync<T>(
        HttpMethod method, string requestUri, object body, CancellationToken cancellationToken)
    {
        using var content = MultipartFormDataBuilder.Build(body);
        using var request = new HttpRequestMessage(method, requestUri) { Content = content };
        using var response = await Http.SendAsync(request, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response, cancellationToken).ConfigureAwait(false);
        return (await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken).ConfigureAwait(false))!;
    }

    /// <summary>For write operations whose response has no body (e.g. wish list item acquire).</summary>
    private protected async Task SendFormNoResultAsync(
        HttpMethod method, string requestUri, object? body, CancellationToken cancellationToken)
    {
        using var content = body is null ? null : MultipartFormDataBuilder.Build(body);
        using var request = new HttpRequestMessage(method, requestUri) { Content = content };
        using var response = await Http.SendAsync(request, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>For POST operations whose parameters are query-string only (e.g. pull list series add).</summary>
    private protected async Task<T> PostNoBodyAsync<T>(string requestUri, CancellationToken cancellationToken)
    {
        using var response = await Http.PostAsync(requestUri, content: null, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response, cancellationToken).ConfigureAwait(false);
        return (await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken).ConfigureAwait(false))!;
    }

    private protected async Task DeleteAsync(string requestUri, CancellationToken cancellationToken)
    {
        using var response = await Http.DeleteAsync(requestUri, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response, cancellationToken).ConfigureAwait(false);
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        string? detail = null;
        try
        {
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (doc.RootElement.ValueKind == JsonValueKind.Object && doc.RootElement.TryGetProperty("detail", out var detailElement))
            {
                detail = detailElement.GetString();
            }
        }
        catch (JsonException)
        {
            // Body wasn't JSON (or was empty) -- fall through with no detail.
        }

        throw new MetronApiException(response.StatusCode, detail);
    }
}
