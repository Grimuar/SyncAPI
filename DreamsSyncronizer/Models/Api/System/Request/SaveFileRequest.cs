using System.Text.Json.Serialization;

namespace DreamsSyncronizer.Models.Api.System.Request;

public class SaveFileRequest

    {
        [JsonPropertyName("file")]
        public string FileText { get; set; }
        
        [JsonPropertyName("key")]
        public string Key { get; set; }

    
}
