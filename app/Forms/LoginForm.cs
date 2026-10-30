using System.Security.Authentication;
using MiniErp.App.Controls.Auth;
using MiniErp.App.Models;
using MiniErp.App.Services;

namespace MiniErp.App.Forms;

internal sealed class LoginForm : Form
{
    private readonly AuthService _authService;
    private readonly LoginControl _loginControl = new();

    public event EventHandler<LoginSucceededEventArgs>? LoginSucceeded;

    public LoginForm(AuthService authService)
    {
        _authService = authService;
        Text = "เข้าสู่ระบบ - Mini ERP";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(520, 600);
        ClientSize = new Size(600, 680);
        Controls.Add(_loginControl);
        _loginControl.LoginSubmitted += LoginControl_LoginSubmitted;
    }

    public void ShowError(string message) => _loginControl.ShowError(message);

    private async void LoginControl_LoginSubmitted(object? sender, LoginSubmittedEventArgs e)
    {
        _loginControl.SetBusy(true);
        try
        {
            var session = await _authService.LoginAsync(e.Name, e.Password);
            if (!IsDisposed && !Disposing)
            {
                LoginSucceeded?.Invoke(this, new LoginSucceededEventArgs(session));
            }
        }
        catch (Exception exception) when (
            exception is AuthenticationException
                or HttpRequestException
                or InvalidDataException
                or TaskCanceledException)
        {
            if (!IsDisposed)
            {
                _loginControl.SetBusy(false);
                _loginControl.ShowError(exception.Message);
            }
        }
    }
}

internal sealed class LoginSucceededEventArgs(AuthSession session) : EventArgs
{
    public AuthSession Session { get; } = session;
}
