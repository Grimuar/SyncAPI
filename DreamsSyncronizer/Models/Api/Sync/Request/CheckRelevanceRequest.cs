using System.Text.Json.Serialization;
using DreamsSyncronizer.Infrastructure.JsonConverters;
using DreamsSyncronizer.Models.Db.Sync;

namespace DreamsSyncronizer.Models.Api.Sync.Request;

public sealed class CheckRelevanceRequest
{
    /// <summary>
    /// ключ для связи пользователей
    /// </summary>
    [JsonPropertyName("linkId")]
    public string LinkId { get; set; }

    [JsonPropertyName("items")]
    public List<CheckRequestItem> Items { get; set; } = new();
}

public sealed class CheckRequestItem
{
    /// <summary>
    /// раздел данных
    /// </summary>
    [JsonPropertyName("key")]
    public string Key { get; set; }

    /// <summary>
    /// md5
    /// </summary>
    [JsonPropertyName("hash")]
    public string Hash { get; set; }

    /// <summary>
    /// дата создания данных на МП (UTC)
    /// </summary>
    [JsonPropertyName("timestamp")]
    [JsonConverter(typeof(DateTimeToStringConverter))]
    public DateTime Timestamp { get; set; }
}