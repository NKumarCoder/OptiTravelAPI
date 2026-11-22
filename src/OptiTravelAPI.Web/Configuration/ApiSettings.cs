namespace OptiTravelAPI.Web.Configuration;

/// <summary>
/// Swagger and API documentation configuration options.
/// </summary>
public class SwaggerOptions
{
    public const string SectionName = "Swagger";

    public bool Enabled { get; set; } = true;
    public string Title { get; set; } = "OptiTravelAPI - Travel Consolidation Platform";
    public string Version { get; set; } = "v1";
    public string Description { get; set; } = "Modern, modular travel consolidation API supporting Bus, Hotel, Flight, Cab, and Transfers services across multiple upstream suppliers.";
    public string RoutePrefix { get; set; } = "swagger";
}
