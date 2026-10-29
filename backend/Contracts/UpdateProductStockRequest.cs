using System.ComponentModel.DataAnnotations;

namespace MiniErp.Api.Contracts;

public class UpdateProductStockRequest
{
    [Range(0, int.MaxValue)]
    public int StockQuantity { get; init; }
}
