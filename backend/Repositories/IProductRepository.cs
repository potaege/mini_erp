using MiniErp.Api.Models;
using MiniErp.Api.Contracts;

namespace MiniErp.Api.Repositories;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllAsync(
        string? search,
        CancellationToken cancellationToken);

    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<Product?> GetByIdForUpdateAsync(int id, CancellationToken cancellationToken);

    Task<IReadOnlyList<ProductLogResponse>> GetLogsAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<ProductLogResponse>> GetLogsByProductIdAsync(
        int productId,
        CancellationToken cancellationToken);

    Task<bool> SkuExistsAsync(
        string sku,
        int? excludedProductId,
        CancellationToken cancellationToken);

    Task AddWithLogsAsync(
        Product product,
        IReadOnlyCollection<ProductLog> logs,
        CancellationToken cancellationToken);

    Task SaveChangesWithLogsAsync(
        IReadOnlyCollection<ProductLog> logs,
        CancellationToken cancellationToken);

    Task DeleteWithLogAsync(
        Product product,
        ProductLog log,
        CancellationToken cancellationToken);
}
