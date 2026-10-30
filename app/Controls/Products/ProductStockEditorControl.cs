using MiniErp.App.Models;

namespace MiniErp.App.Controls.Products;

internal sealed class ProductStockEditorControl : UserControl
{
    private readonly Label _selectedProductLabel = new();
    private readonly Button _editButton = new();
    private Product? _selectedProduct;
    private bool _isUpdating;

    public event EventHandler<ProductStockUpdateEventArgs>? StockUpdateRequested;

    public ProductStockEditorControl()
    {
        Dock = DockStyle.Bottom;
        Height = 92;
        Padding = new Padding(24, 12, 24, 12);
        BackColor = Color.White;

        _selectedProductLabel.Text = "เลือกสินค้าเพื่อปรับจำนวนคงเหลือ";
        _selectedProductLabel.Dock = DockStyle.Left;
        _selectedProductLabel.Width = 360;
        _selectedProductLabel.TextAlign = ContentAlignment.MiddleLeft;
        _selectedProductLabel.Font = ProductUi.Bold9;

        _editButton.Text = "ปรับจำนวนคงเหลือ";
        _editButton.Dock = DockStyle.Right;
        _editButton.Width = 150;
        _editButton.Enabled = false;
        _editButton.Click += (_, _) =>
        {
            if (_selectedProduct is null)
            {
                return;
            }

            using var dialog = new ProductStockDialog(_selectedProduct);
            if (dialog.ShowDialog(FindForm()) == DialogResult.OK)
            {
                StockUpdateRequested?.Invoke(
                    this,
                    new ProductStockUpdateEventArgs(_selectedProduct, dialog.StockQuantity));
            }
        };

        Controls.Add(_editButton);
        Controls.Add(_selectedProductLabel);
    }

    public void SetSelectedProduct(Product? product)
    {
        _selectedProduct = product;
        _editButton.Enabled = product is not null && !_isUpdating;
        _selectedProductLabel.Text = product is null
            ? "เลือกสินค้าเพื่อปรับจำนวนคงเหลือ"
            : $"{product.Name} ({product.Sku})";
    }

    public void SetUpdating(bool isUpdating)
    {
        _isUpdating = isUpdating;
        SetSelectedProduct(_selectedProduct);
    }
}

internal sealed class ProductStockUpdateEventArgs(Product product, int stockQuantity) : EventArgs
{
    public Product Product { get; } = product;
    public int StockQuantity { get; } = stockQuantity;
}
