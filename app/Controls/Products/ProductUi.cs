namespace MiniErp.App.Controls.Products;

internal static class ProductUi
{
    public static readonly Color Ink = Color.FromArgb(32, 48, 71);
    public static readonly Color Muted = Color.FromArgb(71, 85, 105);
    public static readonly Color Surface = Color.FromArgb(245, 247, 250);
    public static readonly Font Regular9 = new("Segoe UI", 9);
    public static readonly Font Regular10 = new("Segoe UI", 10);
    public static readonly Font Bold9 = new("Segoe UI", 9, FontStyle.Bold);
    public static readonly Font Bold16 = new("Segoe UI", 16, FontStyle.Bold);

    public static Label MakeLabel(
        string text,
        DockStyle dock,
        int width,
        ContentAlignment align = ContentAlignment.MiddleLeft,
        Font? font = null,
        Color? foreColor = null) =>
        new()
        {
            Text = text,
            Dock = dock,
            Width = width,
            TextAlign = align,
            Font = font ?? Regular9,
            ForeColor = foreColor ?? Muted
        };
}
