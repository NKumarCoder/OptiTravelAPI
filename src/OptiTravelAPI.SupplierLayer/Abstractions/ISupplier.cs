namespace OptiTravelAPI.SupplierLayer.Abstractions;

/// <summary>
/// Common base abstraction for all travel service suppliers.
/// </summary>
public interface ISupplier
{
    /// <summary>
    /// Unique name of the supplier (e.g. "TBO", "RedBus").
    /// </summary>
    string SupplierName { get; }

    /// <summary>
    /// Service category this supplier provides (e.g. "Bus", "Hotel").
    /// </summary>
    string ServiceType { get; }

    /// <summary>
    /// Whether this supplier is enabled for operations.
    /// </summary>
    bool IsEnabled { get; }
}
