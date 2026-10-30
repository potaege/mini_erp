using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MiniErp.App.Models;

namespace MiniErp.App.Services;

internal sealed class ProductsService(
    HttpClient httpClient,
    Uri apiBaseAddress,
    AuthSessionStore sessionStore)
{
    public Task<IReadOnlyList<Product>> SearchAsync(
        string search,
        CancellationToken cancellationToken = default) =>
        ExecuteAsync(
            () => SearchCoreAsync(search, cancellationToken),
            cancellationToken,
            "เชื่อมต่อระบบหลังบ้านไม่ได้ กรุณาตรวจสอบเครือข่ายและ URL ของ API",
            "ระบบหลังบ้านใช้เวลาตอบกลับนานเกินไป กรุณาลองใหม่");

    public Task<Product> UpdateStockAsync(
        int productId,
        int stockQuantity,
        CancellationToken cancellationToken = default)
    {
        if (stockQuantity < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(stockQuantity),
                "จำนวนสินค้าคงเหลือต้องไม่ติดลบ");
        }

        return ExecuteAsync(
            () => UpdateStockCoreAsync(productId, stockQuantity, cancellationToken),
            cancellationToken,
            "เชื่อมต่อระบบหลังบ้านไม่ได้ จึงบันทึกจำนวนสินค้าไม่สำเร็จ",
            "ระบบหลังบ้านใช้เวลาตอบกลับนานเกินไป ไม่ได้ยืนยันการบันทึก กรุณาตรวจสอบจำนวนก่อนลองใหม่");
    }

    private async Task<IReadOnlyList<Product>> SearchCoreAsync(
        string search,
        CancellationToken cancellationToken)
    {
        var path = "api/Products";
        if (!string.IsNullOrWhiteSpace(search))
        {
            path += $"?search={Uri.EscapeDataString(search.Trim())}";
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, new Uri(apiBaseAddress, path));
        sessionStore.AddAuthorizationHeader(request);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw CreateApiException(response, "ค้นหาสินค้า");
        }

        try
        {
            return await response.Content.ReadFromJsonAsync<List<Product>>(
                       cancellationToken: cancellationToken)
                   ?? throw new InvalidDataException("API ส่งรายการสินค้าว่างในรูปแบบที่ไม่ถูกต้อง");
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException("อ่านรายการสินค้าจาก API ไม่สำเร็จ", exception);
        }
    }

    private async Task<Product> UpdateStockCoreAsync(
        int productId,
        int stockQuantity,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Put,
            new Uri(apiBaseAddress, $"api/Products/{productId}/stock"))
        {
            Content = JsonContent.Create(new UpdateProductStockRequest(stockQuantity))
        };
        sessionStore.AddAuthorizationHeader(request);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw CreateApiException(response, "ปรับจำนวนสินค้าคงเหลือ");
        }

        try
        {
            return await response.Content.ReadFromJsonAsync<Product>(
                       cancellationToken: cancellationToken)
                   ?? throw new InvalidDataException("API ไม่ได้ส่งข้อมูลสินค้าที่ปรับจำนวนแล้วกลับมา");
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException("อ่านผลการปรับจำนวนสินค้าจาก API ไม่สำเร็จ", exception);
        }
    }

    private static async Task<T> ExecuteAsync<T>(
        Func<Task<T>> operation,
        CancellationToken cancellationToken,
        string connectionError,
        string timeoutError)
    {
        try
        {
            return await operation();
        }
        catch (HttpRequestException exception)
        {
            throw new ApiException(0, connectionError, exception);
        }
        catch (OperationCanceledException exception) when (!cancellationToken.IsCancellationRequested)
        {
            throw new ApiException(0, timeoutError, exception);
        }
        catch (InvalidDataException exception)
        {
            throw new ApiException(0, exception.Message, exception);
        }
    }

    private static ApiException CreateApiException(
        HttpResponseMessage response,
        string operation)
    {
        var statusCode = (int)response.StatusCode;
        var message = response.StatusCode switch
        {
            HttpStatusCode.Unauthorized => "เซสชันหมดอายุแล้ว กรุณาเข้าสู่ระบบใหม่",
            HttpStatusCode.Forbidden => "บัญชีนี้ไม่มีสิทธิ์ดำเนินการดังกล่าว",
            HttpStatusCode.NotFound => "ไม่พบสินค้าที่เลือก อาจถูกลบไปแล้ว",
            HttpStatusCode.BadRequest => $"ข้อมูลไม่ถูกต้อง ไม่สามารถ{operation}ได้",
            HttpStatusCode.Conflict => $"ข้อมูลมีการเปลี่ยนแปลง ไม่สามารถ{operation}ได้",
            HttpStatusCode.UnprocessableEntity => $"ข้อมูลไม่ผ่านการตรวจสอบ ไม่สามารถ{operation}ได้",
            _ when statusCode >= 500 => $"ระบบหลังบ้านขัดข้อง ไม่สามารถ{operation}ได้ในขณะนี้",
            _ => $"ไม่สามารถ{operation}ได้ (รหัส {statusCode})"
        };

        return new ApiException(statusCode, message);
    }
}
