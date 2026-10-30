namespace MiniErp.Api.Contracts;

public record LoginResponse(string AccessToken, string TokenType, DateTime ExpiresAtUtc);
