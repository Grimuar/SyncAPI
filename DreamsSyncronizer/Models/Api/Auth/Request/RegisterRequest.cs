using System.Text.Json.Serialization;

namespace DreamsSyncronizer.Models.Api.Auth.Request;

public sealed class RegisterRequest
{
    [JsonPropertyName("login")]
    public string Login { get; set; }

    [JsonPropertyName("password")]
    public string Password { get; set; }
}