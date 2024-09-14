using System.Text.Json.Serialization;
using DreamsSyncronizer.Infrastructure.JsonConverters;

namespace DreamsSyncronizer.Models.Api.Sync.Response;

public sealed class UploadResponse
{
    [JsonPropertyName("items")]
    public UploadResponseItem[] Items { get; set; }
}

public sealed class UploadResponseItem
{
    [JsonPropertyName("key")]
    public string Key { get; set; }

    [JsonPropertyName("timestamp")]
    [JsonConverter(typeof(DateTimeToStringConverter))]
    public DateTime Timestamp { get; set; }
}