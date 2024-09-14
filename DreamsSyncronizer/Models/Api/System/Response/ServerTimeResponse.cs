using System.Text.Json.Serialization;
using DreamsSyncronizer.Infrastructure.JsonConverters;

namespace DreamsSyncronizer.Models.Api.System.Response;

public sealed record ServerTimeResponse
{
    [JsonPropertyName("serverTimeUtc")]
    [JsonConverter(typeof(DateTimeToStringConverter))]
    public DateTime ServerTimeUtc { get; set; }
}