using System.Text.Json.Serialization;

namespace MiniErp.App.Models;

internal sealed record LoginRequest(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("password")] string Password);
