namespace DoctorsManagementSystem.Desktop.Services;

/// <summary>
/// Wraps any failure talking to the backend (network, non-success status,
/// malformed response) into a single, UI-friendly exception type so
/// ViewModels don't need to know about HttpRequestException/JsonException/etc.
/// </summary>
public class ApiException : Exception
{
    public int? StatusCode { get; }

    public ApiException(string message, int? statusCode = null, Exception? innerException = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
    }
}