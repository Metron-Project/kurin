using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Metron.Api.Serialization;

/// <summary>
/// The Metron API serializes DecimalField values (e.g. purchase_price) as JSON strings rather
/// than numbers. This converter reads/writes a nullable decimal using that string representation.
/// </summary>
public sealed class DecimalStringConverter : JsonConverter<decimal?>
{
    public override decimal? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetDecimal();
        }

        var text = reader.GetString();
        if (string.IsNullOrEmpty(text))
        {
            return null;
        }

        return decimal.Parse(text, NumberStyles.Number, CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, decimal? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteStringValue(value.Value.ToString(CultureInfo.InvariantCulture));
        }
    }
}
