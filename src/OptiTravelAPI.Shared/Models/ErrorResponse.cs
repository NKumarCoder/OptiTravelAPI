namespace OptiTravelAPI.Shared.Models;

/// <summary>
/// Standard error response structure returned on failures.
/// </summary>
public class ErrorResponse
{
    public string TraceId { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? ErrorCode { get; set; }
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
    public IDictionary<string, string[]>? ValidationErrors { get; set; }
}
