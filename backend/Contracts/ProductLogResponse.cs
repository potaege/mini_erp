using MiniErp.Api.Models;

namespace MiniErp.Api.Contracts;

public sealed record ProductLogResponse(
    long Id,
    int ProductId,
    string? ProductName,
    string? ProductSku,
    int UserId,
    string UserName,
    ProductLogType Type,
    string? OldValue,
    string? NewValue,
    int? ChangeQuantity,
    DateTime CreatedAt);
