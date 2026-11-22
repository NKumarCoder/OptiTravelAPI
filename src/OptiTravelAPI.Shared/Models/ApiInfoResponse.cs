namespace OptiTravelAPI.Shared.Models;

/// <summary>
/// Response model for the API information endpoint.
/// </summary>
public class ApiInfoResponse
{
    public string Name { get; set; } = "OptiTravelAPI";
    public string Version { get; set; } = "1.0.0";
    public string Description { get; set; } = "Modern Modular Travel Consolidation API Platform";
    public string Runtime { get; set; } = ".NET 10";
    public string Environment { get; set; } = string.Empty;
    public DateTime ServerTimeUtc { get; set; } = DateTime.UtcNow;
    public string TraceId { get; set; } = string.Empty;
    public IReadOnlyList<string> SupportedServices { get; set; } = Array.Empty<string>();
}
