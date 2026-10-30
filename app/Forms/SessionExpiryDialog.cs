namespace MiniErp.App.Forms;

internal sealed class SessionExpiryDialog : Form
{
    private readonly DateTimeOffset _expiresAtUtc;
    private readonly Label _messageLabel = new();
    private readonly System.Windows.Forms.Timer _timer = new() { Interval = 500 };

    public SessionExpiryDialog(DateTimeOffset expiresAtUtc)
    {
        _expiresAtUtc = expiresAtUtc;
        Text = "Session ใกล้หมดอายุ";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(420, 170);

        _messageLabel.Dock = DockStyle.Top;
        _messageLabel.Height = 90;
        _messageLabel.Padding = new Padding(20);
        _messageLabel.TextAlign = ContentAlignment.MiddleCenter;
        _messageLabel.Font = new Font("Segoe UI", 11);

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 58,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(12, 8, 12, 8)
        };
        var stayButton = new Button
        {
            Text = "อยู่ต่อ",
            Width = 110,
            Height = 36,
            DialogResult = DialogResult.Yes
        };
        var leaveButton = new Button
        {
            Text = "ไม่อยู่ต่อ",
            Width = 110,
            Height = 36,
            DialogResult = DialogResult.No
        };
        buttons.Controls.Add(stayButton);
        buttons.Controls.Add(leaveButton);

        Controls.Add(_messageLabel);
        Controls.Add(buttons);
        AcceptButton = stayButton;
        CancelButton = leaveButton;

        _timer.Tick += Timer_Tick;
        FormClosed += (_, _) => _timer.Stop();
        UpdateMessage();
        _timer.Start();
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        if (DateTimeOffset.UtcNow >= _expiresAtUtc)
        {
            _timer.Stop();
            DialogResult = DialogResult.Cancel;
            Close();
            return;
        }

        UpdateMessage();
    }

    private void UpdateMessage()
    {
        var remaining = _expiresAtUtc - DateTimeOffset.UtcNow;
        var totalSeconds = Math.Max(0, (int)Math.Ceiling(remaining.TotalSeconds));
        var countdown = $"{totalSeconds / 60:00}:{totalSeconds % 60:00}";
        _messageLabel.Text = $"Session จะหมดอายุใน {countdown}\nต้องการอยู่ต่อไหม?";
    }
}
