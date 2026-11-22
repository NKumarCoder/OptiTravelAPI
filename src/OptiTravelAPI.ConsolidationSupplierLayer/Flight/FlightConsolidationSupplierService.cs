using OptiTravelAPI.ConsolidationLayer.Flight;

namespace OptiTravelAPI.ConsolidationSupplierLayer.Flight;

public interface IFlightConsolidationSupplierService
{
}

public class FlightConsolidationSupplierService : IFlightConsolidationSupplierService
{
    private readonly IFlightConsolidationEngine _flightEngine;

    public FlightConsolidationSupplierService(IFlightConsolidationEngine flightEngine)
    {
        _flightEngine = flightEngine;
    }
}
