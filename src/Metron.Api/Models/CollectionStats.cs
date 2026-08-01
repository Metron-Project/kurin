using System.Text.Json.Serialization;

namespace Metron.Api.Models;

/// <summary>
/// Response of GET /api/collection/stats/. Hand-written (not generated) because the schema
/// defines this response as an inline anonymous object rather than a named component.
/// </summary>
public sealed class CollectionStats
{
    /// <summary>Total number of distinct collection items.</summary>
    [JsonPropertyName("total_items")]
    public int? TotalItems { get; set; }

    /// <summary>Total quantity across all collection items (accounting for duplicates).</summary>
    [JsonPropertyName("total_quantity")]
    public int? TotalQuantity { get; set; }

    /// <summary>Total value of the collection, as a decimal string.</summary>
    [JsonPropertyName("total_value")]
    public string? TotalValue { get; set; }

    /// <summary>Number of issues marked as read.</summary>
    [JsonPropertyName("read_count")]
    public int? ReadCount { get; set; }

    /// <summary>Number of issues not marked as read.</summary>
    [JsonPropertyName("unread_count")]
    public int? UnreadCount { get; set; }

    /// <summary>Item counts broken down by book format (print, digital, both).</summary>
    [JsonPropertyName("by_format")]
    public List<CollectionStatsByFormat>? ByFormat { get; set; }
}

/// <summary>Item count for a single book format, part of <see cref="CollectionStats"/>.</summary>
public sealed class CollectionStatsByFormat
{
    /// <summary>The book format this count applies to (e.g. "PRINT", "DIGITAL", "BOTH").</summary>
    [JsonPropertyName("book_format")]
    public string? BookFormat { get; set; }

    /// <summary>Number of items in this format.</summary>
    [JsonPropertyName("count")]
    public int? Count { get; set; }
}
