using System.Text.Json.Serialization;

namespace DreamsSyncronizer.Models.Api.System.Request;

public class CountRequest
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }
    
    }
