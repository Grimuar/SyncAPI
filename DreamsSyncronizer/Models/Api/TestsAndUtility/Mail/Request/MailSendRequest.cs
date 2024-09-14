using System.Text.Json.Serialization;

namespace DreamsSyncronizer.Models.Api.TestsAndUtility.Mail.Request;

public class MailSendRequest
{
    [JsonPropertyName ("email")]
    public string Email { get; set; }

    [JsonPropertyName ("subject")]
    public string Subject { get; set; }
    
    [JsonPropertyName ("message")]
    public string Message { get; set; }
}