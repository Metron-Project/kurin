using System.Net;

namespace Metron.Api.Exceptions;

/// <summary>Thrown when the Metron API returns a non-success status code.</summary>
public class MetronApiException : Exception
{
    public HttpStatusCode StatusCode { get; }

    /// <summary>The "detail" field from the API's JSON error body, if present.</summary>
    public string? Detail { get; }

    public MetronApiException(HttpStatusCode statusCode, string? detail)
        : base(BuildMessage(statusCode, detail))
    {
        StatusCode = statusCode;
        Detail = detail;
    }

    private static string BuildMessage(HttpStatusCode statusCode, string? detail) =>
        detail is null
            ? $"Metron API request failed with status {(int)statusCode} ({statusCode})."
            : $"Metron API request failed with status {(int)statusCode} ({statusCode}): {detail}";
}
