using System.ComponentModel.DataAnnotations;

namespace MiniErp.Api.Contracts;

public class LoginRequest
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public required string Name { get; init; }

    [Required]
    [StringLength(30, MinimumLength = 1)]
    public required string Password { get; init; }
}
