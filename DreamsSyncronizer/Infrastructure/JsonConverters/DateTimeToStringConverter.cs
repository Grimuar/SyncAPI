using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DreamsSyncronizer.Infrastructure.JsonConverters;

public sealed class DateTimeToStringConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var result = DateTime.MinValue;

        var dateString = reader.GetString();
        if (!string.IsNullOrWhiteSpace(dateString)
            && DateTime.TryParseExact(dateString, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
        {
            result = parsedDate;
        }

        return result;
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(Format));
    }

    private static readonly string Format = "yyyy-MM-ddTHH:mm:ss";
}