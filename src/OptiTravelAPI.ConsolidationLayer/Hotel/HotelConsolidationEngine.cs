using Microsoft.Extensions.Logging;
using OptiTravelAPI.SupplierLayer.Abstractions;

namespace OptiTravelAPI.ConsolidationLayer.Hotel;

public class HotelConsolidationEngine : IHotelConsolidationEngine
{
    private readonly ISupplierResolver _supplierResolver;
    private readonly ILogger<HotelConsolidationEngine> _logger;

    public HotelConsolidationEngine(ISupplierResolver supplierResolver, ILogger<HotelConsolidationEngine> logger)
    {
        _supplierResolver = supplierResolver;
        _logger = logger;
    }
}
