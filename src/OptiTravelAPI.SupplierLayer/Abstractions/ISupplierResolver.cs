using OptiTravelAPI.SupplierLayer.Abstractions;

namespace OptiTravelAPI.SupplierLayer.Abstractions;

/// <summary>
/// Service contract to resolve supplier implementations dynamically at runtime.
/// </summary>
public interface ISupplierResolver
{
    /// <summary>
    /// Resolves a specific supplier by name and service type.
    /// </summary>
    TSupplier GetSupplier<TSupplier>(string supplierName) where TSupplier : class, ISupplier;

    /// <summary>
    /// Gets all registered and enabled suppliers for a specific travel service type.
    /// </summary>
    IReadOnlyList<TSupplier> GetSuppliers<TSupplier>() where TSupplier : class, ISupplier;
}
