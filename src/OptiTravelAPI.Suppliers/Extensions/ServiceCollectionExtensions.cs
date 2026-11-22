using Microsoft.Extensions.DependencyInjection;
using OptiTravelAPI.SupplierLayer.Abstractions;
using OptiTravelAPI.SupplierLayer.Bus;
using OptiTravelAPI.Suppliers.Bus.Tbo;

namespace OptiTravelAPI.Suppliers.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all active suppliers with the dependency injection container.
    /// </summary>
    public static IServiceCollection AddSuppliers(this IServiceCollection services)
    {
        // Bus Suppliers
        services.AddScoped<TboBusSupplier>();
        services.AddScoped<IBusSupplier>(sp => sp.GetRequiredService<TboBusSupplier>());
        services.AddScoped<ISupplier>(sp => sp.GetRequiredService<TboBusSupplier>());

        // Future Hotel, Flight, Cab, Transfers suppliers will be registered here

        return services;
    }
}
