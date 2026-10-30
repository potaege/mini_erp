using System.Text.Json.Serialization;

namespace MiniErp.App.Models;

internal sealed class LoginResponse
{
    [JsonPropertyName("accessToken")]
    public string? AccessToken { get; init; }

    [JsonPropertyName("tokenType")]
    public string? TokenType { get; init; }

    [JsonPropertyName("expiresAtUtc")]
    public DateTimeOffset ExpiresAtUtc { get; init; }
}
