using System.ComponentModel.DataAnnotations;

namespace MiniErp.Api.Contracts;

public class UpdateProductRequest
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public required string Name { get; init; }

    [Required]
    [StringLength(50, MinimumLength = 1)]
    public required string Sku { get; init; }

    [Range(typeof(decimal), "0.01", "9999999999.99")]
    public decimal Price { get; init; }
}
