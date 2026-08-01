using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Metron.Api.Http;

/// <summary>
/// Builds a multipart/form-data body from a generated model by reflecting over its
/// [JsonPropertyName]-decorated properties. All writes go through multipart/form-data (rather
/// than JSON) because several Metron resources only accept multipart/urlencoded bodies -- see
/// the plan's "Every create/update operation accepts multipart/form-data" note.
/// </summary>
public static class MultipartFormDataBuilder
{
    /// <summary>Serializes a generated model's properties into a multipart/form-data body.</summary>
    public static MultipartFormDataContent Build(object model)
    {
        var content = new MultipartFormDataContent();

        foreach (var property in model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var value = property.GetValue(model);
            if (value is null)
            {
                continue;
            }

            var fieldName = property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? property.Name;

            if (value is string str)
            {
                content.Add(new StringContent(str), fieldName);
                continue;
            }

            if (value is IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    if (item is null)
                    {
                        continue;
                    }

                    var formattedItem = FormatScalar(item);
                    if (formattedItem is not null)
                    {
                        content.Add(new StringContent(formattedItem), fieldName);
                    }
                }

                continue;
            }

            var formatted = FormatScalar(value);
            if (formatted is not null)
            {
                content.Add(new StringContent(formatted), fieldName);
            }
        }

        return content;
    }

    private static string? FormatScalar(object value) => value switch
    {
        bool b => b ? "true" : "false",
        DateOnly d => d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
        DateTimeOffset dt => dt.ToString("o", CultureInfo.InvariantCulture),
        DateTime dt => dt.ToString("o", CultureInfo.InvariantCulture),
        decimal m => m.ToString(CultureInfo.InvariantCulture),
        double d => d.ToString(CultureInfo.InvariantCulture),
        float f => f.ToString(CultureInfo.InvariantCulture),
        int or long or short => Convert.ToString(value, CultureInfo.InvariantCulture),
        Enum e => FormatEnum(e),
        string s => s,
        _ => value.ToString(),
    };

    private static string FormatEnum(Enum value)
    {
        var underlying = Enum.GetUnderlyingType(value.GetType());
        return underlying == typeof(int) || underlying == typeof(long) || underlying == typeof(short)
            ? Convert.ToInt64(value, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture)
            : value.ToString();
    }
}
