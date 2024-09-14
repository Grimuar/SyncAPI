using System.Text.Json.Serialization;
using DreamsSyncronizer.Infrastructure.JsonConverters;

namespace DreamsSyncronizer.Models.Api.Sync.Response;

public sealed record CheckDataResponse
{
    [JsonPropertyName("keys")]
    public List<string> Keys { get; set; }
    
    [JsonPropertyName("lastDate")]
    [JsonConverter(typeof(DateTimeToStringConverter))]
    public DateTime LastDate { get; set; }
}