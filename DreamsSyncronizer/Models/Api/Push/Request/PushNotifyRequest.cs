using System.Text.Json.Serialization;

namespace DreamsSyncronizer.Models.Api.Push.Request;

public sealed class PushNotifyRequest
{
    /// <summary>
    /// тип устройства 
    /// </summary>
    [JsonPropertyName("deviceType")]
    public int DeviceType{ get; set; }
    
    /// <summary>
    /// токен устройства для отправки Push-уведомлений
    /// </summary>
    [JsonPropertyName("deviceToken")]
    public string DeviceToken { get; set; }
    
    /// <summary>
    /// уникальный id устройства
    /// </summary>
    [JsonPropertyName("deviceId")]
    public string DeviceId { get; set; }
}