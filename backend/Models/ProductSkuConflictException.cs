namespace MiniErp.Api.Models;

public sealed class ProductSkuConflictException(string sku)
    : Exception($"A product with SKU '{sku}' already exists.");
