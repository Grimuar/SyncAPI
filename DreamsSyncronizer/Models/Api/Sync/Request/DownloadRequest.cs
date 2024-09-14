using System.Text.Json.Serialization;

namespace DreamsSyncronizer.Models.Api.Sync.Request;

public sealed class DownloadRequest
{
    [JsonPropertyName("linkId")]
    public string LinkId { get; set; }

    [JsonPropertyName("keys")]
    public string[] Keys { get; set; }
}