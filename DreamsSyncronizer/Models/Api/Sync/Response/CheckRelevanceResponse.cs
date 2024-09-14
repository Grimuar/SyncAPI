using System.Text.Json.Serialization;

namespace DreamsSyncronizer.Models.Api.Sync.Response;

public sealed class CheckRelevanceResponse
{
    [JsonPropertyName("actual")]
    public string[] Actual { get; set; }
    
    [JsonPropertyName("forUpload")]
    public string[] ForUpload { get; set; }

    [JsonPropertyName("forDownload")]
    public string[] ForDownload { get; set; }
}