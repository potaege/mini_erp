using System.Globalization;
using MiniErp.Api.Contracts;
using MiniErp.Api.Models;
using MiniErp.Api.Repositories;

namespace MiniErp.Api.Services;

public class ProductService(IProductRepository productRepository) : IProductService
{
    public Task<IReadOnlyList<Product>> GetAllAsync(
        string? search,
        CancellationToken cancellationToken)
    {
        return productRepository.GetAllAsync(search, cancellationToken);
    }

    public Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return productRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IReadOnlyList<ProductLogResponse>> GetLogsAsync(
        CancellationToken cancellationToken)
    {
        var result = await productRepository.GetLogsAsync(cancellationToken);
        return ToLogResponse(result);
    }

    public async Task<IReadOnlyList<ProductLogResponse>> GetLogsByProductIdAsync(
        int productId,
        CancellationToken cancellationToken)
    {
        var result = await productRepository.GetLogsByProductIdAsync(
            productId,
            cancellationToken);
        return ToLogResponse(result);
    }

    public async Task<Product> CreateAsync(
        CreateProductRequest request,
        int userId,
        CancellationToken cancellationToken)
    {
        if (await productRepository.SkuExistsAsync(request.Sku, null, cancellationToken))
        {
            throw new ProductSkuConflictException(request.Sku);
        }

        var product = new Product
        {
            Name = request.Name,
            Sku = request.Sku,
            Price = request.Price,
            StockQuantity = request.StockQuantity
        };

        var logs = new List<ProductLog>
        {
            CreateLog(product, userId, ProductLogType.Create, newValue: product.Sku)
        };
        if (product.StockQuantity > 0)
        {
            logs.Add(CreateLog(
                product,
                userId,
                ProductLogType.StockIn,
                oldValue: "0",
                newValue: product.StockQuantity.ToString(CultureInfo.InvariantCulture),
                changeQuantity: product.StockQuantity));
        }

        await productRepository.AddWithLogsAsync(product, logs, cancellationToken);
        return product;
    }

    public async Task<Product?> UpdateAsync(
        int id,
        UpdateProductRequest request,
        int userId,
        CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdForUpdateAsync(id, cancellationToken);

        if (product is null)
        {
            return null;
        }

        if (await productRepository.SkuExistsAsync(request.Sku, id, cancellationToken))
        {
            throw new ProductSkuConflictException(request.Sku);
        }

        var logs = new List<ProductLog>();
        if (!string.Equals(product.Name, request.Name, StringComparison.Ordinal))
        {
            logs.Add(CreateLog(
                product,
                userId,
                ProductLogType.Name,
                oldValue: product.Name,
                newValue: request.Name));
            product.Name = request.Name;
        }

        if (!string.Equals(product.Sku, request.Sku, StringComparison.Ordinal))
        {
            logs.Add(CreateLog(
                product,
                userId,
                ProductLogType.Sku,
                oldValue: product.Sku,
                newValue: request.Sku));
            product.Sku = request.Sku;
        }

        if (product.Price != request.Price)
        {
            logs.Add(CreateLog(
                product,
                userId,
                ProductLogType.Price,
                oldValue: product.Price.ToString("0.00", CultureInfo.InvariantCulture),
                newValue: request.Price.ToString("0.00", CultureInfo.InvariantCulture)));
            product.Price = request.Price;
        }

        if (logs.Count > 0)
        {
            await productRepository.SaveChangesWithLogsAsync(logs, cancellationToken);
        }

        return product;
    }

    public async Task<Product?> UpdateStockAsync(
        int id,
        UpdateProductStockRequest request,
        int userId,
        CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdForUpdateAsync(id, cancellationToken);

        if (product is null)
        {
            return null;
        }

        var changeQuantity = request.StockQuantity - product.StockQuantity;
        if (changeQuantity == 0)
        {
            return product;
        }

        var oldQuantity = product.StockQuantity;
        product.StockQuantity = request.StockQuantity;
        var log = CreateLog(
            product,
            userId,
            changeQuantity > 0 ? ProductLogType.StockIn : ProductLogType.StockOut,
            oldValue: oldQuantity.ToString(CultureInfo.InvariantCulture),
            newValue: request.StockQuantity.ToString(CultureInfo.InvariantCulture),
            changeQuantity: changeQuantity);

        await productRepository.SaveChangesWithLogsAsync([log], cancellationToken);
        return product;
    }

    public async Task<bool> DeleteAsync(
        int id,
        int userId,
        CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdForUpdateAsync(id, cancellationToken);

        if (product is null)
        {
            return false;
        }

        var log = CreateLog(
            product,
            userId,
            ProductLogType.Delete,
            oldValue: product.Sku,
            newValue: product.Name);
        await productRepository.DeleteWithLogAsync(product, log, cancellationToken);
        return true;
    }

    private static ProductLog CreateLog(
        Product product,
        int userId,
        ProductLogType type,
        string? oldValue = null,
        string? newValue = null,
        int? changeQuantity = null)
    {
        return new ProductLog
        {
            ProductId = product.Id,
            UserId = userId,
            Type = type,
            OldValue = oldValue,
            NewValue = newValue,
            ChangeQuantity = changeQuantity
        };
    }

    private static IReadOnlyList<ProductLogResponse> ToLogResponse(
        IReadOnlyList<ProductLog> logs)
    {
        return logs
            .Select(log => new ProductLogResponse(
                log.Id,
                log.ProductId,
                log.UserId,
                log.Type,
                log.OldValue,
                log.NewValue,
                log.ChangeQuantity,
                log.CreatedAt))
            .ToList();
    }
}
