using System.Text.Json;

namespace MiniErp.App.Services;

internal sealed record AppSettings(Uri ApiBaseAddress, TimeSpan WarningLeadTime)
{
    public static AppSettings Load()
    {
        var settingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        using var settings = JsonDocument.Parse(File.ReadAllText(settingsPath));

        var apiBaseUrl = Environment.GetEnvironmentVariable("MINIERP_API_BASE_URL")
            ?? settings.RootElement.GetProperty("Api").GetProperty("BaseUrl").GetString();
        if (string.IsNullOrWhiteSpace(apiBaseUrl)
            || !Uri.TryCreate(apiBaseUrl, UriKind.Absolute, out var apiBaseAddress)
            || (apiBaseAddress.Scheme != Uri.UriSchemeHttp && apiBaseAddress.Scheme != Uri.UriSchemeHttps))
        {
            throw new InvalidDataException("Api:BaseUrl ต้องเป็น URL แบบ http หรือ https");
        }

        if (apiBaseAddress.Scheme == Uri.UriSchemeHttp && !apiBaseAddress.IsLoopback)
        {
            throw new InvalidDataException("การเชื่อมต่อ backend ที่ไม่ใช่เครื่องนี้ต้องใช้ https");
        }

        var warningMinutes = settings.RootElement
            .GetProperty("Authentication")
            .GetProperty("WarningMinutes")
            .GetInt32();
        if (warningMinutes is < 1 or > 1440)
        {
            throw new InvalidDataException("Authentication:WarningMinutes ต้องอยู่ระหว่าง 1 ถึง 1440 นาที");
        }

        var normalizedBaseAddress = apiBaseAddress.AbsoluteUri.EndsWith('/')
            ? apiBaseAddress
            : new Uri($"{apiBaseAddress.AbsoluteUri}/", UriKind.Absolute);

        return new AppSettings(normalizedBaseAddress, TimeSpan.FromMinutes(warningMinutes));
    }
}
