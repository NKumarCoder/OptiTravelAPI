using Microsoft.Extensions.DependencyInjection;
using OptiTravelAPI.ConsolidationLayer.Bus;
using OptiTravelAPI.ConsolidationLayer.Cab;
using OptiTravelAPI.ConsolidationLayer.Flight;
using OptiTravelAPI.ConsolidationLayer.Hotel;
using OptiTravelAPI.ConsolidationLayer.Transfers;

namespace OptiTravelAPI.ConsolidationLayer.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all ConsolidationLayer business orchestration engines.
    /// </summary>
    public static IServiceCollection AddConsolidationLayer(this IServiceCollection services)
    {
        services.AddScoped<IBusConsolidationEngine, BusConsolidationEngine>();
        services.AddScoped<IHotelConsolidationEngine, HotelConsolidationEngine>();
        services.AddScoped<IFlightConsolidationEngine, FlightConsolidationEngine>();
        services.AddScoped<ICabConsolidationEngine, CabConsolidationEngine>();
        services.AddScoped<ITransfersConsolidationEngine, TransfersConsolidationEngine>();

        return services;
    }
}
