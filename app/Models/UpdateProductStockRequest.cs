using System.Text.Json.Serialization;

namespace MiniErp.App.Models;

internal sealed record UpdateProductStockRequest(
    [property: JsonPropertyName("stockQuantity")] int StockQuantity);
