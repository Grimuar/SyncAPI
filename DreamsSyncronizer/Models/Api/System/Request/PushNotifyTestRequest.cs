using System.Text.Json.Serialization;

namespace DreamsSyncronizer.Models.Api.System.Request;


public sealed class PushNotifyTestRequest
{
    /// <summary>
    /// ID пользовательского устройства <device_token>
    /// </summary>
    [JsonPropertyName("deviceToken")]
    public string DeviceToken{ get; set; }
    
    /// <summary>
    /// Токен для APNS, игнорируется если используется сертификат
    /// </summary>
    [JsonPropertyName("providerToken")]
    public string BearerProviderToken { get; set; }
    
    /// <summary>
    /// Сообщение в пуш
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; }
}