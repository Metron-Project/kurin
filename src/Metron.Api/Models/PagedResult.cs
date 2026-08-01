using System.Text.Json.Serialization;

namespace Metron.Api.Models;

/// <summary>
/// Generic replacement for the schema's 15 structurally-identical Paginated*List wrapper types
/// (count/next/previous/results).
/// </summary>
public sealed class PagedResult<T>
{
    /// <summary>Total number of items across all pages.</summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>URL of the next page, or null if this is the last page.</summary>
    [JsonPropertyName("next")]
    public string? Next { get; set; }

    /// <summary>URL of the previous page, or null if this is the first page.</summary>
    [JsonPropertyName("previous")]
    public string? Previous { get; set; }

    /// <summary>The items on this page.</summary>
    [JsonPropertyName("results")]
    public List<T> Results { get; set; } = [];
}
