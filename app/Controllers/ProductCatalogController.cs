using MiniErp.App.Controls.Products;
using MiniErp.App.Services;

namespace MiniErp.App.Controllers;

internal sealed class ProductCatalogController : IDisposable
{
    private readonly ProductsService _productsService;
    private readonly ProductCatalogControl _view;
    private readonly CancellationTokenSource _lifetime = new();
    private CancellationTokenSource? _searchCancellation;
    private bool _disposed;

    public event EventHandler<SessionEndedEventArgs>? SessionEnded;

    public ProductCatalogController(ProductsService productsService, ProductCatalogControl view)
    {
        _productsService = productsService;
        _view = view;
        _view.SearchRequested += View_SearchRequested;
        _view.StockUpdateRequested += View_StockUpdateRequested;
    }

    public void Load() => _ = SearchAsync(string.Empty);

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _view.SearchRequested -= View_SearchRequested;
        _view.StockUpdateRequested -= View_StockUpdateRequested;
        _lifetime.Cancel();
        _searchCancellation?.Cancel();
        _lifetime.Dispose();
    }

    private async void View_SearchRequested(object? sender, ProductSearchEventArgs e)
    {
        await SearchAsync(e.SearchTerm);
    }

    private async Task<bool> SearchAsync(string search, string? successMessage = null)
    {
        _searchCancellation?.Cancel();
        var cancellation = CancellationTokenSource.CreateLinkedTokenSource(_lifetime.Token);
        _searchCancellation = cancellation;
        _view.SetLoading(true);

        try
        {
            return await ExecuteAsync(
                token => _productsService.SearchAsync(search, token),
                products =>
                {
                    _view.ShowProducts(products);
                    if (successMessage is not null)
                    {
                        _view.ShowInformation(successMessage);
                    }
                },
                cancellation.Token);
        }
        finally
        {
            var isCurrentSearch = ReferenceEquals(_searchCancellation, cancellation);
            if (isCurrentSearch)
            {
                _searchCancellation = null;
            }

            cancellation.Dispose();
            if (isCurrentSearch && !_view.IsDisposed)
            {
                _view.SetLoading(false);
            }
        }
    }

    private async void View_StockUpdateRequested(
        object? sender,
        ProductStockUpdateEventArgs e)
    {
        _view.SetUpdatingStock(true);
        try
        {
            var productName = string.Empty;
            var saved = await ExecuteAsync(
                token => _productsService.UpdateStockAsync(e.Product.Id, e.StockQuantity, token),
                updatedProduct => productName = updatedProduct.Name,
                _lifetime.Token);

            if (saved && !_lifetime.IsCancellationRequested && !_view.IsDisposed)
            {
                await SearchAsync(
                    _view.SearchTerm,
                    $"บันทึกจำนวนคงเหลือของ {productName} แล้ว และโหลดข้อมูลล่าสุดเรียบร้อย");
            }
        }
        finally
        {
            if (!_view.IsDisposed)
            {
                _view.SetUpdatingStock(false);
            }
        }
    }

    private async Task<bool> ExecuteAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        Action<T> onSuccess,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await operation(cancellationToken);
            if (!cancellationToken.IsCancellationRequested && !_view.IsDisposed)
            {
                onSuccess(result);
                return true;
            }

            return false;
        }
        catch (ApiException exception)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                HandleError(exception);
            }

            return false;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
    }

    private void HandleError(ApiException exception)
    {
        if (_view.IsDisposed)
        {
            return;
        }

        if (exception.StatusCode == (int)System.Net.HttpStatusCode.Unauthorized)
        {
            SessionEnded?.Invoke(this, new SessionEndedEventArgs(exception.Message));
            return;
        }

        _view.ShowError(exception.Message);
    }
}

internal sealed class SessionEndedEventArgs(string? message) : EventArgs
{
    public string? Message { get; } = message;
}
