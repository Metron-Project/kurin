using System.Text.Json.Serialization;

namespace Metron.Api.Models;

/// <summary>
/// Response of GET /api/collection/stats/. Hand-written (not generated) because the schema
/// defines this response as an inline anonymous object rather than a named component.
/// </summary>
public sealed class CollectionStats
{
    [JsonPropertyName("total_items")]
    public int? TotalItems { get; set; }

    [JsonPropertyName("total_quantity")]
    public int? TotalQuantity { get; set; }

    [JsonPropertyName("total_value")]
    public string? TotalValue { get; set; }

    [JsonPropertyName("read_count")]
    public int? ReadCount { get; set; }

    [JsonPropertyName("unread_count")]
    public int? UnreadCount { get; set; }

    [JsonPropertyName("by_format")]
    public List<CollectionStatsByFormat>? ByFormat { get; set; }
}

public sealed class CollectionStatsByFormat
{
    [JsonPropertyName("book_format")]
    public string? BookFormat { get; set; }

    [JsonPropertyName("count")]
    public int? Count { get; set; }
}
