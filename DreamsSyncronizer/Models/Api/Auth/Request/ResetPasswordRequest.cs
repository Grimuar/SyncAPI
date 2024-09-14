using System.Text.Json.Serialization;

namespace DreamsSyncronizer.Models.Api.Auth.Request;

public class ResetPasswordRequest
{
    [JsonPropertyName ("email")]
    public string Email { get; set; }
}