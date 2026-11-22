using OptiTravelAPI.SupplierLayer.Abstractions;

namespace OptiTravelAPI.SupplierLayer.Abstractions;

/// <summary>
/// Factory interface for instantiating or acquiring supplier instances.
/// </summary>
public interface ISupplierFactory<out TSupplier> where TSupplier : ISupplier
{
    TSupplier Create(string supplierName);
}
