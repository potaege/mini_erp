using MiniErp.Api.Models;

namespace MiniErp.Api.Contracts;

public sealed record ProductLogResponse(
    long Id,
    int ProductId,
    int UserId,
    ProductLogType Type,
    string? OldValue,
    string? NewValue,
    int? ChangeQuantity,
    DateTime CreatedAt);
