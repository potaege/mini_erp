namespace MiniErp.App.Services;

internal sealed class ApiException(
    int statusCode,
    string message,
    Exception? innerException = null) : Exception(message, innerException)
{
    public int StatusCode { get; } = statusCode;
}
