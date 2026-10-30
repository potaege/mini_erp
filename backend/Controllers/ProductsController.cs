using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniErp.Api.Contracts;
using MiniErp.Api.Models;
using MiniErp.Api.Services;

namespace MiniErp.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ProductsController(IProductService productService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetAll(
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        var products = await productService.GetAllAsync(search, cancellationToken);
        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetById(int id, CancellationToken cancellationToken)
    {
        var product = await productService.GetByIdAsync(id, cancellationToken);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpGet("logs")]
    public async Task<ActionResult<IReadOnlyList<ProductLogResponse>>> GetLogs(
        CancellationToken cancellationToken)
    {
        var logs = await productService.GetLogsAsync(cancellationToken);
        return Ok(logs);
    }

    [HttpGet("{id:int}/logs")]
    public async Task<ActionResult<IReadOnlyList<ProductLogResponse>>> GetLogsByProductId(
        int id,
        CancellationToken cancellationToken)
    {
        var logs = await productService.GetLogsByProductIdAsync(id, cancellationToken);
        return Ok(logs);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Create(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        try
        {
            var product = await productService.CreateAsync(request, userId, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }
        catch (ProductSkuConflictException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Product>> Update(
        int id,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        try
        {
            var product = await productService.UpdateAsync(id, request, userId, cancellationToken);
            return product is null ? NotFound() : Ok(product);
        }
        catch (ProductSkuConflictException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [HttpPut("{id:int}/stock")]
    public async Task<ActionResult<Product>> UpdateStock(
        int id,
        UpdateProductStockRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var product = await productService.UpdateStockAsync(
            id,
            request,
            userId,
            cancellationToken);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var deleted = await productService.DeleteAsync(id, userId, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    private bool TryGetUserId(out int userId)
    {
        return int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
    }

}
