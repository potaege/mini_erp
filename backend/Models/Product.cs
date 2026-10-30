namespace MiniErp.Api.Models;

public class Product
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Sku { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

}
