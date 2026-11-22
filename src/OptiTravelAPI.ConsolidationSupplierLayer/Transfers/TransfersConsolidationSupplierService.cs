using OptiTravelAPI.ConsolidationLayer.Transfers;

namespace OptiTravelAPI.ConsolidationSupplierLayer.Transfers;

public interface ITransfersConsolidationSupplierService
{
}

public class TransfersConsolidationSupplierService : ITransfersConsolidationSupplierService
{
    private readonly ITransfersConsolidationEngine _transfersEngine;

    public TransfersConsolidationSupplierService(ITransfersConsolidationEngine transfersEngine)
    {
        _transfersEngine = transfersEngine;
    }
}
