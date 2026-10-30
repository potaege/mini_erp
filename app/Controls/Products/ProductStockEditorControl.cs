using MiniErp.App.Models;

namespace MiniErp.App.Controls.Products;

internal sealed class ProductStockEditorControl : UserControl
{
    private readonly Label _selectedProductLabel = new();
    private readonly NumericUpDown _stockQuantityInput = new();
    private readonly Button _saveButton = new();
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

        _saveButton.Text = "บันทึกจำนวน";
        _saveButton.Dock = DockStyle.Right;
        _saveButton.Width = 120;
        _saveButton.Enabled = false;
        _saveButton.Click += (_, _) =>
        {
            if (_selectedProduct is not null)
            {
                StockUpdateRequested?.Invoke(
                    this,
                    new ProductStockUpdateEventArgs(
                        _selectedProduct,
                        Decimal.ToInt32(_stockQuantityInput.Value)));
            }
        };

        _stockQuantityInput.Dock = DockStyle.Right;
        _stockQuantityInput.Width = 130;
        _stockQuantityInput.Minimum = 0;
        _stockQuantityInput.Maximum = int.MaxValue;
        _stockQuantityInput.Font = ProductUi.Regular10;
        _stockQuantityInput.Enabled = false;

        Controls.Add(_saveButton);
        Controls.Add(_stockQuantityInput);
        Controls.Add(ProductUi.MakeLabel("จำนวนคงเหลือ", DockStyle.Right, 115, ContentAlignment.MiddleCenter));
        Controls.Add(_selectedProductLabel);
    }

    public void SetSelectedProduct(Product? product)
    {
        _selectedProduct = product;
        _stockQuantityInput.Enabled = product is not null && !_isUpdating;
        _saveButton.Enabled = product is not null && !_isUpdating;
        _selectedProductLabel.Text = product is null
            ? "เลือกสินค้าเพื่อปรับจำนวนคงเหลือ"
            : $"{product.Name} ({product.Sku})";

        if (product is not null)
        {
            _stockQuantityInput.Value = Math.Clamp(
                product.StockQuantity,
                (int)_stockQuantityInput.Minimum,
                (int)_stockQuantityInput.Maximum);
        }
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
