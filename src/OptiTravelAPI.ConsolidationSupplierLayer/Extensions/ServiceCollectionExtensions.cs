using Microsoft.Extensions.DependencyInjection;
using OptiTravelAPI.ConsolidationSupplierLayer.Bus;
using OptiTravelAPI.ConsolidationSupplierLayer.Cab;
using OptiTravelAPI.ConsolidationSupplierLayer.Flight;
using OptiTravelAPI.ConsolidationSupplierLayer.Hotel;
using OptiTravelAPI.ConsolidationSupplierLayer.Transfers;

namespace OptiTravelAPI.ConsolidationSupplierLayer.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all ConsolidationSupplierLayer gateway services.
    /// </summary>
    public static IServiceCollection AddConsolidationSupplierLayer(this IServiceCollection services)
    {
        services.AddScoped<IBusConsolidationSupplierService, BusConsolidationSupplierService>();
        services.AddScoped<IHotelConsolidationSupplierService, HotelConsolidationSupplierService>();
        services.AddScoped<IFlightConsolidationSupplierService, FlightConsolidationSupplierService>();
        services.AddScoped<ICabConsolidationSupplierService, CabConsolidationSupplierService>();
        services.AddScoped<ITransfersConsolidationSupplierService, TransfersConsolidationSupplierService>();

        return services;
    }
}
