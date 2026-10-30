using System.Text.Json.Serialization;

namespace MiniErp.App.Models;

internal sealed class Product
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("sku")]
    public string Sku { get; init; } = string.Empty;

    [JsonPropertyName("price")]
    public decimal Price { get; init; }

    [JsonPropertyName("stockQuantity")]
    public int StockQuantity { get; init; }

    [JsonPropertyName("isActive")]
    public bool IsActive { get; init; }
}
