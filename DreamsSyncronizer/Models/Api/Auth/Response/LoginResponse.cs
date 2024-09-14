using System.Text.Json.Serialization;

namespace DreamsSyncronizer.Models.Api.Auth.Response;

public sealed record LoginResponse(string AccessToken, string Login, string Uuid)
{
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; } = AccessToken;

    [JsonPropertyName("login")]
    public string Login { get; set; } = Login;

    [JsonPropertyName("uuid")]
    public string Uuid { get; set; } = Uuid;
}