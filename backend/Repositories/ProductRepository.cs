using Microsoft.EntityFrameworkCore;
using MiniErp.Api.Data;
using MiniErp.Api.Models;

namespace MiniErp.Api.Repositories;

public class ProductRepository(AppDbContext dbContext) : IProductRepository
{
    public async Task<IReadOnlyList<Product>> GetAllAsync(
        string? search,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Products.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(product =>
                product.Sku.Contains(search) || product.Name.Contains(search));
        }

        return await query
            .OrderBy(product => product.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return dbContext.Products
            .AsNoTracking()
            .SingleOrDefaultAsync(product => product.Id == id, cancellationToken);
    }

    public Task<Product?> GetByIdForUpdateAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return dbContext.Products
            .SingleOrDefaultAsync(product => product.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<ProductLog>> GetLogsAsync(
        CancellationToken cancellationToken)
    {
        return await dbContext.ProductLogs
            .AsNoTracking()
            .OrderByDescending(log => log.CreatedAt)
            .ThenByDescending(log => log.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProductLog>> GetLogsByProductIdAsync(
        int productId,
        CancellationToken cancellationToken)
    {
        return await dbContext.ProductLogs
            .AsNoTracking()
            .Where(log => log.ProductId == productId)
            .OrderByDescending(log => log.CreatedAt)
            .ThenByDescending(log => log.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> SkuExistsAsync(
        string sku,
        int? excludedProductId,
        CancellationToken cancellationToken)
    {
        return dbContext.Products.AnyAsync(
            product => product.Sku == sku
                && (!excludedProductId.HasValue || product.Id != excludedProductId.Value),
            cancellationToken);
    }

    public async Task AddWithLogsAsync(
        Product product,
        IReadOnlyCollection<ProductLog> logs,
        CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database
            .BeginTransactionAsync(cancellationToken);
        await dbContext.Products.AddAsync(product, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        foreach (var log in logs)
        {
            log.ProductId = product.Id;
        }

        await dbContext.ProductLogs.AddRangeAsync(logs, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task SaveChangesWithLogsAsync(
        IReadOnlyCollection<ProductLog> logs,
        CancellationToken cancellationToken)
    {
        await dbContext.ProductLogs.AddRangeAsync(logs, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteWithLogAsync(
        Product product,
        ProductLog log,
        CancellationToken cancellationToken)
    {
        await dbContext.ProductLogs.AddAsync(log, cancellationToken);
        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

}
