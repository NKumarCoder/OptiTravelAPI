using Microsoft.Extensions.Logging;
using OptiTravelAPI.ConsolidationLayer.Hotel;

namespace OptiTravelAPI.ConsolidationSupplierLayer.Hotel;

public interface IHotelConsolidationSupplierService
{
}

public class HotelConsolidationSupplierService : IHotelConsolidationSupplierService
{
    private readonly IHotelConsolidationEngine _hotelEngine;
    private readonly ILogger<HotelConsolidationSupplierService> _logger;

    public HotelConsolidationSupplierService(IHotelConsolidationEngine hotelEngine, ILogger<HotelConsolidationSupplierService> logger)
    {
        _hotelEngine = hotelEngine;
        _logger = logger;
    }
}
