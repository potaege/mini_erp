namespace MiniErp.App.Models;

internal sealed record AuthSession(
    string UserName,
    string AccessToken,
    string TokenType,
    DateTimeOffset ExpiresAtUtc);
