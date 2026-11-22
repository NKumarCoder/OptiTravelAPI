namespace OptiTravelAPI.Shared.Models;

/// <summary>
/// Response model for the health check endpoint.
/// </summary>
public class HealthResponse
{
    public string Status { get; set; } = "Healthy";
    public string Service { get; set; } = "OptiTravelAPI";
    public string Environment { get; set; } = string.Empty;
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
    public string TraceId { get; set; } = string.Empty;
}
