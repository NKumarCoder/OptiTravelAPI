namespace OptiTravelAPI.Shared.Models;

/// <summary>
/// Configuration model for multi-supplier settings across all travel services.
/// Format in appsettings.json:
/// {
///   "Suppliers": {
///     "Bus": {
///       "TBO": {
///         "Enabled": true,
///         "BaseUrl": "https://api.tbo.com",
///         "Username": "...",
///         "Password": "...",
///         "TimeoutSeconds": 60
///       }
///     }
///   }
/// }
/// </summary>
public class SupplierConfiguration : Dictionary<string, Dictionary<string, SupplierItemConfig>>
{
    public SupplierConfiguration() : base(StringComparer.OrdinalIgnoreCase)
    {
    }

    /// <summary>
    /// Safely retrieves supplier configuration by service category and supplier name.
    /// </summary>
    public SupplierItemConfig? GetSupplierConfig(string serviceType, string supplierName)
    {
        if (TryGetValue(serviceType, out var suppliers) &&
            suppliers.TryGetValue(supplierName, out var config))
        {
            return config;
        }
        return null;
    }
}

public class SupplierItemConfig
{
    public bool Enabled { get; set; } = true;
    public int Priority { get; set; } = 1;
    public int TimeoutSeconds { get; set; } = 60;
    public string? BaseUrl { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
}
