using Microsoft.Extensions.DependencyInjection;
using OptiTravelAPI.SupplierLayer.Abstractions;
using OptiTravelAPI.SupplierLayer.Factories;

namespace OptiTravelAPI.SupplierLayer.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers SupplierLayer core abstractions and resolvers.
    /// </summary>
    public static IServiceCollection AddSupplierLayer(this IServiceCollection services)
    {
        services.AddScoped<ISupplierResolver, SupplierResolver>();
        return services;
    }
}
