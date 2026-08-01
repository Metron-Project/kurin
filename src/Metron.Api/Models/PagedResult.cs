using System.Text.Json.Serialization;

namespace Metron.Api.Models;

/// <summary>
/// Generic replacement for the schema's 15 structurally-identical Paginated*List wrapper types
/// (count/next/previous/results).
/// </summary>
public sealed class PagedResult<T>
{
    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("next")]
    public string? Next { get; set; }

    [JsonPropertyName("previous")]
    public string? Previous { get; set; }

    [JsonPropertyName("results")]
    public List<T> Results { get; set; } = [];
}
