using OptiTravelAPI.ConsolidationLayer.Cab;

namespace OptiTravelAPI.ConsolidationSupplierLayer.Cab;

public interface ICabConsolidationSupplierService
{
}

public class CabConsolidationSupplierService : ICabConsolidationSupplierService
{
    private readonly ICabConsolidationEngine _cabEngine;

    public CabConsolidationSupplierService(ICabConsolidationEngine cabEngine)
    {
        _cabEngine = cabEngine;
    }
}
