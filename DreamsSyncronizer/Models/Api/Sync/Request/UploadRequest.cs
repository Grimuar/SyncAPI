using System.Text.Json.Serialization;

namespace DreamsSyncronizer.Models.Api.Sync.Request;

public sealed class UploadRequest
{
    /// <summary>
    /// ключ для связи пользователей
    /// </summary>
    [JsonPropertyName("linkId")]
    public string LinkId { get; set; }

    [JsonPropertyName("items")]
    public UploadRequestItem[] Items { get; set; }

    [JsonPropertyName("dbVersion")]
    public int DbVersion { get; set; }
}

public sealed class UploadRequestItem
{
    /// <summary>
    /// раздел данных
    /// </summary>
    [JsonPropertyName("key")]
    public string Key { get; set; }

    /// <summary>
    /// сами данные
    /// </summary>
    [JsonPropertyName("jsonData")]
    public string JsonData { get; set; }

    /// <summary>
    /// md5
    /// </summary>
    [JsonPropertyName("hash")]
    public string Hash { get; set; }
}