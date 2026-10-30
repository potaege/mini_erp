using MiniErp.App.Controls.Products;
using MiniErp.App.Controllers;
using MiniErp.App.Models;
using MiniErp.App.Services;

namespace MiniErp.App.Forms;

internal sealed class ProductForm : Form
{
    private readonly AuthSession _session;
    private readonly TimeSpan _warningLeadTime;
    private readonly ProductCatalogControl _catalogView;
    private readonly ProductCatalogController _catalogController;
    private readonly System.Windows.Forms.Timer _sessionTimer = new() { Interval = 1000 };
    private SessionExpiryDialog? _sessionExpiryDialog;
    private bool _warningShown;
    private bool _sessionEnded;

    public event EventHandler<SessionEndedEventArgs>? SessionEnded;

    public ProductForm(
        AuthSession session,
        ProductsService productsService,
        TimeSpan warningLeadTime)
    {
        _session = session;
        _warningLeadTime = warningLeadTime;
        _catalogView = new ProductCatalogControl(session.UserName);
        _catalogController = new ProductCatalogController(productsService, _catalogView);
        _catalogController.SessionEnded += CatalogController_SessionEnded;

        Text = "รายการสินค้า - Mini ERP";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(760, 520);
        ClientSize = new Size(960, 640);
        Controls.Add(_catalogView);

        _catalogView.LogoutRequested += (_, _) => EndSession();
        _sessionTimer.Tick += SessionTimer_Tick;
        Shown += ProductForm_Shown;
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _catalogController.Dispose();
        _sessionTimer.Stop();
        _sessionTimer.Dispose();
        _sessionExpiryDialog?.Close();
        base.OnFormClosed(e);
    }

    private void ProductForm_Shown(object? sender, EventArgs e)
    {
        _sessionTimer.Start();
        SessionTimer_Tick(this, EventArgs.Empty);
        _catalogController.Load();
    }

    private void CatalogController_SessionEnded(object? sender, SessionEndedEventArgs e) =>
        EndSession(e.Message);

    private void SessionTimer_Tick(object? sender, EventArgs e)
    {
        var remaining = _session.ExpiresAtUtc - DateTimeOffset.UtcNow;
        if (remaining <= TimeSpan.Zero)
        {
            EndSession("Session หมดอายุแล้ว กรุณาเข้าสู่ระบบใหม่");
            return;
        }

        if (!_warningShown && remaining <= _warningLeadTime)
        {
            _warningShown = true;
            ShowSessionExpiryDialog();
        }
    }

    private void ShowSessionExpiryDialog()
    {
        if (_sessionExpiryDialog is { IsDisposed: false })
        {
            return;
        }

        var dialog = new SessionExpiryDialog(_session.ExpiresAtUtc);
        _sessionExpiryDialog = dialog;
        dialog.FormClosed += (_, _) =>
        {
            if (ReferenceEquals(_sessionExpiryDialog, dialog))
            {
                _sessionExpiryDialog = null;
            }

            if (dialog.DialogResult == DialogResult.No)
            {
                EndSession();
            }
        };
        dialog.Show(this);
    }

    private void EndSession(string? message = null)
    {
        if (_sessionEnded)
        {
            return;
        }

        _sessionEnded = true;
        _sessionTimer.Stop();
        _sessionExpiryDialog?.Close();
        SessionEnded?.Invoke(this, new SessionEndedEventArgs(message));
    }
}
