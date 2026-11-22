using OptiTravelAPI.ConsolidationLayer.Extensions;
using OptiTravelAPI.ConsolidationSupplierLayer.Extensions;
using OptiTravelAPI.Shared.Models;
using OptiTravelAPI.SupplierLayer.Extensions;
using OptiTravelAPI.Suppliers.Extensions;

namespace OptiTravelAPI.Web.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all 4 architectural layers and shared infrastructure.
    /// </summary>
    public static IServiceCollection AddOptiTravelLayers(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Supplier Layer (Abstractions, Factories, Resolvers)
        services.AddSupplierLayer();

        // 2. Suppliers (Individual Implementations)
        services.AddSuppliers();

        // 3. Consolidation Layer (Business Orchestration & Aggregation Engines)
        services.AddConsolidationLayer();

        // 4. Consolidation Supplier Layer (API Gateway / Service Translators)
        services.AddConsolidationSupplierLayer();

        // HttpClient factory for future supplier communications
        services.AddHttpClient();

        // Bind supplier configuration
        services.Configure<SupplierConfiguration>(configuration.GetSection("Suppliers"));

        return services;
    }
}
