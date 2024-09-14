
using System.Text.Json.Serialization;

namespace DreamsSyncronizer.Models.Api;

public class ErrorResponse
{
    public ErrorResponse(string errorCode)
    {
        ErrorCode = errorCode;
    }

    [JsonPropertyName("errorCode")]
    public string ErrorCode { get; }
}