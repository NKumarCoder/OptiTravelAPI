using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using OptiTravelAPI.Shared.Constants;
using OptiTravelAPI.Shared.Exceptions;
using OptiTravelAPI.Shared.Models;
using OptiTravelAPI.SupplierLayer.Abstractions;
using OptiTravelAPI.SupplierLayer.Bus;
using OptiTravelAPI.SupplierLayer.Factories;
using OptiTravelAPI.Suppliers.Bus.Tbo;
using Xunit;

namespace OptiTravelAPI.Tests.Architecture;

public class SupplierResolverTests
{
    private class DummyHttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => new HttpClient();
    }

    private TboBusSupplier CreateTboSupplier()
    {
        var config = new SupplierConfiguration();
        return new TboBusSupplier(
            new DummyHttpClientFactory(),
            Options.Create(config),
            NullLogger<TboBusSupplier>.Instance);
    }

    private IServiceProvider CreateServiceProvider() =>
        new ServiceCollection().BuildServiceProvider();

    [Fact]
    public void GetSupplier_ResolvesRegisteredSupplier_CaseInsensitive()
    {
        // Arrange
        var tboSupplier = CreateTboSupplier();
        var suppliers = new ISupplier[] { tboSupplier };
        var resolver = new SupplierResolver(suppliers, CreateServiceProvider(), NullLogger<SupplierResolver>.Instance);

        // Act
        var resolved = resolver.GetSupplier<IBusSupplier>("tbo");

        // Assert
        Assert.NotNull(resolved);
        Assert.Equal(SupplierNames.Tbo, resolved.SupplierName);
        Assert.Equal(ServiceTypes.Bus, resolved.ServiceType);
    }

    [Fact]
    public void GetSupplier_ThrowsNotFoundException_WhenSupplierNotRegistered()
    {
        // Arrange
        var suppliers = Array.Empty<ISupplier>();
        var resolver = new SupplierResolver(suppliers, CreateServiceProvider(), NullLogger<SupplierResolver>.Instance);

        // Act & Assert
        var ex = Assert.Throws<NotFoundException>(() => resolver.GetSupplier<IBusSupplier>("UnknownSupplier"));
        Assert.Contains("UnknownSupplier", ex.Message);
    }

    [Fact]
    public void GetSuppliers_ReturnsOnlyEnabledSuppliersForService()
    {
        // Arrange
        var tboSupplier = CreateTboSupplier();
        var suppliers = new ISupplier[] { tboSupplier };
        var resolver = new SupplierResolver(suppliers, CreateServiceProvider(), NullLogger<SupplierResolver>.Instance);

        // Act
        var busSuppliers = resolver.GetSuppliers<IBusSupplier>();

        // Assert
        Assert.Single(busSuppliers);
        Assert.Equal(SupplierNames.Tbo, busSuppliers[0].SupplierName);
    }
}
