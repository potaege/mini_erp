namespace MiniErp.App.Controls.Products;

internal sealed class ProductSearchControl : UserControl
{
    private readonly TextBox _searchTextBox = new();
    private readonly Button _searchButton = new();

    public event EventHandler<ProductSearchEventArgs>? SearchRequested;

    public ProductSearchControl()
    {
        Dock = DockStyle.Top;
        Height = 72;
        Padding = new Padding(24, 16, 24, 12);
        BackColor = ProductUi.Surface;

        var label = ProductUi.MakeLabel("ค้นหาชื่อหรือรหัสสินค้า", DockStyle.Left, 190, font: ProductUi.Bold9);
        _searchButton.Text = "ค้นหา";
        _searchButton.Dock = DockStyle.Right;
        _searchButton.Width = 100;
        _searchButton.Click += (_, _) => RaiseSearchRequested();

        _searchTextBox.Dock = DockStyle.Fill;
        _searchTextBox.Font = ProductUi.Regular10;
        _searchTextBox.PlaceholderText = "พิมพ์ชื่อหรือรหัสสินค้า";
        _searchTextBox.KeyDown += (_, args) =>
        {
            if (args.KeyCode != Keys.Enter)
            {
                return;
            }

            args.SuppressKeyPress = true;
            RaiseSearchRequested();
        };

        Controls.Add(_searchTextBox);
        Controls.Add(_searchButton);
        Controls.Add(label);
    }

    public string SearchTerm => _searchTextBox.Text.Trim();

    public void SetLoading(bool isLoading)
    {
        _searchTextBox.Enabled = !isLoading;
        _searchButton.Enabled = !isLoading;
    }

    private void RaiseSearchRequested() =>
        SearchRequested?.Invoke(this, new ProductSearchEventArgs(_searchTextBox.Text.Trim()));
}

internal sealed class ProductSearchEventArgs(string searchTerm) : EventArgs
{
    public string SearchTerm { get; } = searchTerm;
}
