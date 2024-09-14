using System.Text.Json.Serialization;

namespace DreamsSyncronizer.Models.Api.Sync.Request;

public sealed record CheckDataRequest
{
    [JsonPropertyName("linkId")]
    public string LinkId { get; set; }
}