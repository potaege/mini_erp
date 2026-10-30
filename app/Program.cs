using System.Text.Json;
using MiniErp.App.Services;

namespace MiniErp.App;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        AppSettings settings;
        try
        {
            settings = AppSettings.Load();
        }
        catch (Exception exception) when (
            exception is IOException
                or InvalidDataException
                or UriFormatException
                or JsonException
                or KeyNotFoundException
                or InvalidOperationException)
        {
            MessageBox.Show(
                $"ตั้งค่าแอปไม่ถูกต้อง: {exception.Message}",
                "Mini ERP",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return;
        }

        using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
        var sessionStore = new AuthSessionStore();
        var authService = new AuthService(httpClient, settings.ApiBaseAddress, sessionStore);
        var productsService = new ProductsService(httpClient, settings.ApiBaseAddress, sessionStore);
        Application.Run(new MiniErpApplicationContext(
            authService,
            productsService,
            sessionStore,
            settings.WarningLeadTime));
    }
}
