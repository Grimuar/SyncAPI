using System.Text.Json.Serialization;

namespace DreamsSyncronizer.Models.Api.Sync.Request;

public sealed class ClearRequest
{
    [JsonPropertyName("linkId")]
    public string LinkId { get; set; }
}