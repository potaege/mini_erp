using System.Net.Http.Headers;
using MiniErp.App.Models;

namespace MiniErp.App.Services;

internal sealed class AuthSessionStore
{
    public AuthSession? Current { get; private set; }

    public void Set(AuthSession session) => Current = session;

    public void Clear() => Current = null;

    public void AddAuthorizationHeader(HttpRequestMessage request)
    {
        if (Current is null)
        {
            throw new InvalidOperationException("ยังไม่มี session ที่เข้าสู่ระบบ");
        }

        request.Headers.Authorization = new AuthenticationHeaderValue(
            Current.TokenType,
            Current.AccessToken);
    }
}
