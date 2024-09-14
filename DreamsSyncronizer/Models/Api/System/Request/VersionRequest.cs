using System.Text.Json.Serialization;

namespace DreamsSyncronizer.Models.Api.System.Request;

public class VersionRequest
{
    [JsonPropertyName("key")]
    public string Key { get; set; }
    
    [JsonPropertyName("version")]
    public string Version { get; set; }
    
    [JsonPropertyName("store")]
    public string Store { get; set; }
}