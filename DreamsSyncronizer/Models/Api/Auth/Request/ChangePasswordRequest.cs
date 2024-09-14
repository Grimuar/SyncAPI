using System.Text.Json.Serialization;

namespace DreamsSyncronizer.Models.Api.Auth.Request;

public class ChangePasswordRequest
{
    [JsonPropertyName("newPassword")]
    public string NewPassword { get; set; }
}