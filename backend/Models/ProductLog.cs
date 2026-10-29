namespace MiniErp.Api.Models;

public class ProductLog
{
    public long Id { get; set; }

    public int ProductId { get; set; }

    public int UserId { get; set; }

    public ProductLogType Type { get; set; }

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public int? ChangeQuantity { get; set; }

    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
}
