using MiniErp.App.Models;

namespace MiniErp.App.Controls.Products;

internal sealed class ProductStockDialog : Form
{
    private readonly NumericUpDown _stockQuantityInput = new();

    public int StockQuantity => Decimal.ToInt32(_stockQuantityInput.Value);

    public ProductStockDialog(Product product)
    {
        Text = "ปรับจำนวนคงเหลือ";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MinimizeBox = false;
        MaximizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(420, 190);
        Font = ProductUi.Regular9;

        var productLabel = ProductUi.MakeLabel(
            $"{product.Name} ({product.Sku})",
            DockStyle.Top,
            0,
            font: ProductUi.Bold9,
            foreColor: ProductUi.Ink);
        productLabel.Padding = new Padding(20, 12, 20, 0);
        productLabel.Height = 48;

        var quantityLabel = ProductUi.MakeLabel(
            "จำนวนคงเหลือใหม่",
            DockStyle.Left,
            150);
        var quantityPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 48,
            Padding = new Padding(20, 4, 20, 4)
        };
        _stockQuantityInput.Dock = DockStyle.Fill;
        _stockQuantityInput.Minimum = 0;
        _stockQuantityInput.Maximum = int.MaxValue;
        _stockQuantityInput.Value = Math.Clamp(product.StockQuantity, 0, int.MaxValue);
        _stockQuantityInput.Font = ProductUi.Regular10;
        quantityPanel.Controls.Add(_stockQuantityInput);
        quantityPanel.Controls.Add(quantityLabel);

        var cancelButton = new Button
        {
            Text = "ยกเลิก",
            DialogResult = DialogResult.Cancel,
            Width = 90,
            Height = 34
        };
        var saveButton = new Button
        {
            Text = "บันทึก",
            DialogResult = DialogResult.OK,
            Width = 90,
            Height = 34
        };
        var buttonsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 58,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(12)
        };
        buttonsPanel.Controls.Add(saveButton);
        buttonsPanel.Controls.Add(cancelButton);

        Controls.Add(quantityPanel);
        Controls.Add(productLabel);
        Controls.Add(buttonsPanel);
        AcceptButton = saveButton;
        CancelButton = cancelButton;
    }
}
