using System.Globalization;
using System.Reflection;

namespace Metron.Api.Http;

/// <summary>
/// Builds a URL query string from a generated filter record by reflecting over its
/// [QueryParameter]-decorated properties. Array-valued filters (e.g. IssueFilter.RoleId) are
/// joined with commas, matching the API's "style: form, explode: false" query parameters.
/// </summary>
public static class QueryStringBuilder
{
    /// <summary>Builds a query string (including the leading "?") from a filter's properties, or "" if none are set.</summary>
    public static string Build(object? filter)
    {
        if (filter is null)
        {
            return string.Empty;
        }

        var pairs = new List<string>();

        foreach (var property in filter.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var attribute = property.GetCustomAttribute<QueryParameterAttribute>();
            if (attribute is null)
            {
                continue;
            }

            var value = property.GetValue(filter);
            if (value is null)
            {
                continue;
            }

            if (value is string str)
            {
                pairs.Add(FormatPair(attribute.Name, str));
                continue;
            }

            if (value is System.Collections.IEnumerable enumerable)
            {
                var items = new List<string>();
                foreach (var item in enumerable)
                {
                    if (item is null)
                    {
                        continue;
                    }

                    var formattedItem = FormatScalar(item);
                    if (formattedItem is not null)
                    {
                        items.Add(formattedItem);
                    }
                }

                if (items.Count > 0)
                {
                    pairs.Add(FormatPair(attribute.Name, string.Join(",", items)));
                }

                continue;
            }

            var formatted = FormatScalar(value);
            if (formatted is not null)
            {
                pairs.Add(FormatPair(attribute.Name, formatted));
            }
        }

        return pairs.Count == 0 ? string.Empty : "?" + string.Join("&", pairs);
    }

    private static string FormatPair(string name, string value) =>
        $"{Uri.EscapeDataString(name)}={Uri.EscapeDataString(value)}";

    private static string? FormatScalar(object value) => value switch
    {
        bool b => b ? "true" : "false",
        DateOnly d => d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
        DateTimeOffset dt => dt.ToString("o", CultureInfo.InvariantCulture),
        double d => d.ToString(CultureInfo.InvariantCulture),
        int or long => Convert.ToString(value, CultureInfo.InvariantCulture),
        string s => s,
        _ => value.ToString(),
    };
}
