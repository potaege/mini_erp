namespace MiniErp.Api.Models;

public class User
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Password { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<ProductLog> ProductLogs { get; } = new List<ProductLog>();
}
