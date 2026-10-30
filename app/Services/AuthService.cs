using System.Net;
using System.Net.Http.Json;
using System.Security.Authentication;
using System.Text.Json;
using MiniErp.App.Models;

namespace MiniErp.App.Services;

internal sealed class AuthService(
    HttpClient httpClient,
    Uri apiBaseAddress,
    AuthSessionStore sessionStore)
{
    public async Task<AuthSession> LoginAsync(
        string name,
        string password,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            new Uri(apiBaseAddress, "api/Auth/login"),
            new LoginRequest(name, password),
            cancellationToken);

        if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
        {
            throw new AuthenticationException("ชื่อผู้ใช้หรือรหัสผ่านไม่ถูกต้อง");
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Login API ตอบกลับสถานะ {(int)response.StatusCode} ({response.ReasonPhrase})");
        }

        LoginResponse? loginResponse;
        try
        {
            loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>(
                cancellationToken: cancellationToken);
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException("รูปแบบคำตอบจาก Login API ไม่ถูกต้อง", exception);
        }

        if (loginResponse is null
            || string.IsNullOrWhiteSpace(loginResponse.AccessToken)
            || string.IsNullOrWhiteSpace(loginResponse.TokenType)
            || loginResponse.ExpiresAtUtc <= DateTimeOffset.UtcNow)
        {
            throw new InvalidDataException("Login API ส่ง token หรือวันหมดอายุไม่ถูกต้อง");
        }

        try
        {
            _ = new System.Net.Http.Headers.AuthenticationHeaderValue(
                loginResponse.TokenType,
                loginResponse.AccessToken);
        }
        catch (FormatException exception)
        {
            throw new InvalidDataException("Login API ส่ง token type หรือ token ที่ไม่ถูกต้อง", exception);
        }

        return new AuthSession(
            name,
            loginResponse.AccessToken,
            loginResponse.TokenType,
            loginResponse.ExpiresAtUtc);
    }

    public void StoreSession(AuthSession session) => sessionStore.Set(session);

    public void ClearSession() => sessionStore.Clear();
}
