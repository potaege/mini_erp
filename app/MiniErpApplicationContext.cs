using MiniErp.App.Controllers;
using MiniErp.App.Forms;
using MiniErp.App.Models;
using MiniErp.App.Services;

namespace MiniErp.App;

internal sealed class MiniErpApplicationContext : ApplicationContext
{
    private readonly AuthService _authService;
    private readonly ProductsService _productsService;
    private readonly AuthSessionStore _sessionStore;
    private readonly TimeSpan _warningLeadTime;

    public MiniErpApplicationContext(
        AuthService authService,
        ProductsService productsService,
        AuthSessionStore sessionStore,
        TimeSpan warningLeadTime)
    {
        _authService = authService;
        _productsService = productsService;
        _sessionStore = sessionStore;
        _warningLeadTime = warningLeadTime;
        ShowLoginForm();
    }

    protected override void ExitThreadCore()
    {
        _sessionStore.Clear();
        base.ExitThreadCore();
    }

    private void ShowLoginForm(string? message = null)
    {
        var previousForm = MainForm;
        var loginForm = new LoginForm(_authService);
        loginForm.LoginSucceeded += LoginForm_LoginSucceeded;

        MainForm = loginForm;
        loginForm.Show();
        if (previousForm is { IsDisposed: false })
        {
            previousForm.Close();
        }

        if (message is not null)
        {
            loginForm.ShowError(message);
        }
    }

    private void LoginForm_LoginSucceeded(object? sender, LoginSucceededEventArgs e)
    {
        if (sender is not LoginForm loginForm || loginForm.IsDisposed)
        {
            return;
        }

        _sessionStore.Set(e.Session);
        var productForm = new ProductForm(e.Session, _productsService, _warningLeadTime);
        productForm.SessionEnded += ProductForm_SessionEnded;

        MainForm = productForm;
        productForm.Show();
        loginForm.Close();
    }

    private void ProductForm_SessionEnded(object? sender, SessionEndedEventArgs e)
    {
        _sessionStore.Clear();
        ShowLoginForm(e.Message);
    }
}
