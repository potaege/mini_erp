namespace MiniErp.App.Controls.Auth;

internal sealed class LoginControl : UserControl
{
    private readonly TextBox _nameTextBox = new();
    private readonly TextBox _passwordTextBox = new();
    private readonly Button _submitButton = new();
    private readonly Label _messageLabel = new();

    public event EventHandler<LoginSubmittedEventArgs>? LoginSubmitted;

    public LoginControl()
    {
        Dock = DockStyle.Fill;
        BackColor = Color.FromArgb(245, 247, 250);

        var host = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(20),
            BackColor = BackColor
        };
        var layout = new TableLayoutPanel
        {
            ColumnCount = 1,
            RowCount = 9,
            Padding = new Padding(30, 20, 30, 20),
            Size = new Size(480, 520),
            BackColor = Color.White
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 18));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 25));

        var title = new Label
        {
            Text = "เข้าสู่ระบบ",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 20, FontStyle.Bold),
            ForeColor = Color.FromArgb(32, 48, 71)
        };
        var subtitle = new Label
        {
            Text = "Mini ERP",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 10),
            ForeColor = Color.FromArgb(100, 116, 139)
        };

        layout.Controls.Add(title, 0, 1);
        layout.Controls.Add(subtitle, 0, 2);
        layout.Controls.Add(CreateFieldLabel("ชื่อผู้ใช้"), 0, 3);

        _nameTextBox.Dock = DockStyle.Fill;
        _nameTextBox.Font = new Font("Segoe UI", 11);
        _nameTextBox.MaxLength = 100;
        layout.Controls.Add(_nameTextBox, 0, 4);
        layout.Controls.Add(CreateFieldLabel("รหัสผ่าน"), 0, 5);

        _passwordTextBox.Dock = DockStyle.Fill;
        _passwordTextBox.Font = new Font("Segoe UI", 11);
        _passwordTextBox.MaxLength = 30;
        _passwordTextBox.UseSystemPasswordChar = true;
        _passwordTextBox.KeyDown += PasswordTextBox_KeyDown;
        layout.Controls.Add(_passwordTextBox, 0, 6);

        _submitButton.Text = "เข้าสู่ระบบ";
        _submitButton.Dock = DockStyle.Fill;
        _submitButton.FlatStyle = FlatStyle.Flat;
        _submitButton.FlatAppearance.BorderSize = 0;
        _submitButton.BackColor = Color.FromArgb(37, 99, 235);
        _submitButton.ForeColor = Color.White;
        _submitButton.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        _submitButton.Click += SubmitButton_Click;
        layout.Controls.Add(_submitButton, 0, 7);

        _messageLabel.Dock = DockStyle.Fill;
        _messageLabel.TextAlign = ContentAlignment.MiddleCenter;
        _messageLabel.ForeColor = Color.Firebrick;
        _messageLabel.Font = new Font("Segoe UI", 9);
        layout.Controls.Add(_messageLabel, 0, 8);

        host.Controls.Add(layout);
        Controls.Add(host);
        Resize += (_, _) =>
        {
            layout.Size = new Size(
                Math.Min(480, Math.Max(360, host.ClientSize.Width - 40)),
                Math.Min(520, Math.Max(420, host.ClientSize.Height - 40)));
            layout.Location = new Point(
                Math.Max(0, (host.ClientSize.Width - layout.Width) / 2),
                Math.Max(0, (host.ClientSize.Height - layout.Height) / 2));
        };
    }

    public void SetBusy(bool isBusy)
    {
        _nameTextBox.Enabled = !isBusy;
        _passwordTextBox.Enabled = !isBusy;
        _submitButton.Enabled = !isBusy;
        _submitButton.Text = isBusy ? "กำลังเข้าสู่ระบบ..." : "เข้าสู่ระบบ";
    }

    public void ShowError(string message)
    {
        _messageLabel.Text = message;
        _passwordTextBox.Clear();
        _passwordTextBox.Focus();
    }

    private static Label CreateFieldLabel(string text) => new()
    {
        Text = text,
        Dock = DockStyle.Fill,
        TextAlign = ContentAlignment.BottomLeft,
        Font = new Font("Segoe UI", 9, FontStyle.Bold),
        ForeColor = Color.FromArgb(51, 65, 85)
    };

    private void SubmitButton_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_nameTextBox.Text))
        {
            ShowError("กรุณากรอกชื่อผู้ใช้");
            _nameTextBox.Focus();
            return;
        }

        if (string.IsNullOrEmpty(_passwordTextBox.Text))
        {
            ShowError("กรุณากรอกรหัสผ่าน");
            _passwordTextBox.Focus();
            return;
        }

        _messageLabel.Text = string.Empty;
        LoginSubmitted?.Invoke(
            this,
            new LoginSubmittedEventArgs(_nameTextBox.Text.Trim(), _passwordTextBox.Text));
    }

    private void PasswordTextBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.SuppressKeyPress = true;
            SubmitButton_Click(sender, EventArgs.Empty);
        }
    }
}

internal sealed class LoginSubmittedEventArgs(string name, string password) : EventArgs
{
    public string Name { get; } = name;
    public string Password { get; } = password;
}
