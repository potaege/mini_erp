using System.ComponentModel;
using MiniErp.App.Models;

namespace MiniErp.App.Controls.Products;

internal sealed class ProductGridControl : UserControl
{
    private readonly DataGridView _grid = new();

    public event EventHandler? SelectionChanged;

    public ProductGridControl()
    {
        Dock = DockStyle.Fill;
        BackColor = Color.White;

        _grid.Dock = DockStyle.Fill;
        _grid.BackgroundColor = Color.White;
        _grid.BorderStyle = BorderStyle.None;
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.AllowUserToResizeRows = false;
        _grid.AutoGenerateColumns = false;
        _grid.ReadOnly = true;
        _grid.MultiSelect = false;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.RowHeadersVisible = false;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grid.ColumnHeadersHeight = 42;
        _grid.Font = ProductUi.Regular9;
        _grid.ColumnHeadersDefaultCellStyle.Font = ProductUi.Bold9;
        _grid.Columns.AddRange(
            CreateTextColumn(nameof(Product.Sku), "รหัสสินค้า", 22),
            CreateTextColumn(nameof(Product.Name), "ชื่อสินค้า", 42),
            CreateTextColumn(nameof(Product.Price), "ราคา", 16, "N2"),
            CreateTextColumn(nameof(Product.StockQuantity), "คงเหลือ", 12),
            new DataGridViewCheckBoxColumn
            {
                DataPropertyName = nameof(Product.IsActive),
                HeaderText = "ใช้งาน",
                FillWeight = 10
            });
        _grid.SelectionChanged += (_, _) => SelectionChanged?.Invoke(this, EventArgs.Empty);
        Controls.Add(_grid);
    }

    public Product? SelectedProduct => _grid.CurrentRow?.DataBoundItem as Product;

    public void ShowProducts(IReadOnlyList<Product> products) =>
        _grid.DataSource = new BindingList<Product>(products.ToList());

    private static DataGridViewTextBoxColumn CreateTextColumn(
        string propertyName,
        string header,
        float fillWeight,
        string? format = null)
    {
        var column = new DataGridViewTextBoxColumn
        {
            DataPropertyName = propertyName,
            HeaderText = header,
            FillWeight = fillWeight
        };
        if (format is not null)
        {
            column.DefaultCellStyle.Format = format;
        }

        return column;
    }
}
