using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OptiTravelAPI.Shared.Exceptions;
using OptiTravelAPI.SupplierLayer.Abstractions;

namespace OptiTravelAPI.SupplierLayer.Factories;

/// <summary>
/// Resolves supplier instances dynamically from the dependency injection container.
/// </summary>
public class SupplierResolver : ISupplierResolver
{
    private readonly IEnumerable<ISupplier> _suppliers;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SupplierResolver> _logger;

    public SupplierResolver(
        IEnumerable<ISupplier> suppliers,
        IServiceProvider serviceProvider,
        ILogger<SupplierResolver> logger)
    {
        _suppliers = suppliers;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public TSupplier GetSupplier<TSupplier>(string supplierName) where TSupplier : class, ISupplier
    {
        // 1. Look in registered ISupplier collection
        var supplier = _suppliers
            .OfType<TSupplier>()
            .FirstOrDefault(s => string.Equals(s.SupplierName, supplierName, StringComparison.OrdinalIgnoreCase));

        // 2. Fall back to specifically registered TSupplier collection
        supplier ??= _serviceProvider.GetServices<TSupplier>()
            .FirstOrDefault(s => string.Equals(s.SupplierName, supplierName, StringComparison.OrdinalIgnoreCase));

        if (supplier == null)
        {
            _logger.LogWarning("Requested supplier '{SupplierName}' for '{SupplierType}' was not found.", supplierName, typeof(TSupplier).Name);
            throw new NotFoundException($"Supplier '{supplierName}' for type '{typeof(TSupplier).Name}' was not found or is not registered.");
        }

        _logger.LogInformation("SupplierLayer: Resolved supplier '{SupplierName}' for '{SupplierType}'.", supplierName, typeof(TSupplier).Name);
        return supplier;
    }

    public IReadOnlyList<TSupplier> GetSuppliers<TSupplier>() where TSupplier : class, ISupplier
    {
        var fromSuppliers = _suppliers.OfType<TSupplier>();
        var fromSpecific = _serviceProvider.GetServices<TSupplier>();

        return fromSuppliers
            .Concat(fromSpecific)
            .Distinct()
            .Where(s => s.IsEnabled)
            .ToList();
    }
}
