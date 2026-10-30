using MiniErp.App.Models;

namespace MiniErp.App.Controls.Products;

internal sealed class ProductCatalogControl : UserControl
{
    private readonly ProductSearchControl _searchControl = new();
    private readonly ProductGridControl _productGrid = new();
    private readonly ProductStockEditorControl _stockEditor = new();
    private readonly Label _statusLabel = new();

    public event EventHandler<ProductSearchEventArgs>? SearchRequested;
    public event EventHandler<ProductStockUpdateEventArgs>? StockUpdateRequested;
    public event EventHandler? LogoutRequested;

    public string SearchTerm => _searchControl.SearchTerm;

    public ProductCatalogControl()
    {
        Dock = DockStyle.Fill;
        BackColor = ProductUi.Surface;

        var header = new Panel
        {
            Dock = DockStyle.Top,
            Height = 68,
            BackColor = Color.White,
            Padding = new Padding(24, 0, 24, 0)
        };
        var title = ProductUi.MakeLabel(
            "รายการสินค้า",
            DockStyle.Left,
            260,
            font: ProductUi.Bold16,
            foreColor: ProductUi.Ink);
        var logoutButton = new Button
        {
            Text = "ออกจากระบบ",
            Dock = DockStyle.Right,
            Width = 120,
            FlatStyle = FlatStyle.Flat,
            ForeColor = ProductUi.Muted
        };
        logoutButton.Click += (_, _) => LogoutRequested?.Invoke(this, EventArgs.Empty);
        header.Controls.Add(logoutButton);
        header.Controls.Add(title);

        var productsBody = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(24, 0, 24, 0),
            BackColor = ProductUi.Surface
        };
        _statusLabel.Dock = DockStyle.Bottom;
        _statusLabel.Height = 36;
        _statusLabel.TextAlign = ContentAlignment.MiddleLeft;
        _statusLabel.Font = ProductUi.Regular9;
        productsBody.Controls.Add(_productGrid);
        productsBody.Controls.Add(_statusLabel);

        Controls.Add(productsBody);
        Controls.Add(_stockEditor);
        Controls.Add(_searchControl);
        Controls.Add(header);

        _searchControl.SearchRequested += (_, args) => SearchRequested?.Invoke(this, args);
        _productGrid.SelectionChanged += (_, _) => _stockEditor.SetSelectedProduct(_productGrid.SelectedProduct);
        _stockEditor.StockUpdateRequested += (_, args) => StockUpdateRequested?.Invoke(this, args);
    }

    public void SetLoading(bool isLoading)
    {
        _searchControl.SetLoading(isLoading);
        if (isLoading)
        {
            SetStatus("กำลังโหลดข้อมูลสินค้า...", ProductUi.Muted);
        }
    }

    public void SetUpdatingStock(bool isUpdating)
    {
        _stockEditor.SetUpdating(isUpdating);
        if (isUpdating)
        {
            SetStatus("กำลังบันทึกจำนวนสินค้าคงเหลือ...", ProductUi.Muted);
        }
    }

    public void ShowProducts(IReadOnlyList<Product> products)
    {
        _productGrid.ShowProducts(products);
        SetStatus(
            products.Count == 0
                ? "ไม่พบสินค้าที่ตรงกับคำค้นหา"
                : $"พบสินค้า {products.Count} รายการ",
            ProductUi.Muted);
        _stockEditor.SetSelectedProduct(_productGrid.SelectedProduct);
    }

    public void ShowError(string message) => SetStatus(message, Color.Firebrick);

    public void ShowInformation(string message) => SetStatus(message, Color.FromArgb(21, 128, 61));

    private void SetStatus(string message, Color color)
    {
        _statusLabel.Text = message;
        _statusLabel.ForeColor = color;
    }
}
