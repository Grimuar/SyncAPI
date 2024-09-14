using System.Text.Json.Serialization;
using DreamsSyncronizer.Infrastructure.JsonConverters;

namespace DreamsSyncronizer.Models.Api.Sync.Response;

public sealed class DownloadResponse
{
    [JsonPropertyName("items")]
    public DownloadResponseItem[] Items { get; set; }
}

public sealed class DownloadResponseItem
{
    [JsonPropertyName("key")]
    public string Key { get; set; }

    [JsonPropertyName("jsonData")]
    public string JsonData { get; set; }

    [JsonPropertyName("timestamp")]
    [JsonConverter(typeof(DateTimeToStringConverter))]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("hash")]
    public string Hash { get; set; }

    [JsonPropertyName("dbVersion")]
    public int DbVersion { get; set; }
}