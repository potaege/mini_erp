using MiniErp.Api.Contracts;
using MiniErp.Api.Models;

namespace MiniErp.Api.Services;

public interface IProductService
{
    Task<IReadOnlyList<Product>> GetAllAsync(
        string? search,
        CancellationToken cancellationToken);

    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<IReadOnlyList<ProductLogResponse>> GetLogsAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<ProductLogResponse>> GetLogsByProductIdAsync(
        int productId,
        CancellationToken cancellationToken);

    Task<Product> CreateAsync(
        CreateProductRequest request,
        int userId,
        CancellationToken cancellationToken);

    Task<Product?> UpdateAsync(
        int id,
        UpdateProductRequest request,
        int userId,
        CancellationToken cancellationToken);

    Task<Product?> UpdateStockAsync(
        int id,
        UpdateProductStockRequest request,
        int userId,
        CancellationToken cancellationToken);

    Task<bool> DeleteAsync(int id, int userId, CancellationToken cancellationToken);
}
