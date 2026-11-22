using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using OptiTravelAPI.Shared.Constants;
using OptiTravelAPI.Shared.Models;
using Xunit;

namespace OptiTravelAPI.Tests.Endpoints;

public class ApiInfoEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ApiInfoEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetApiInfo_ReturnsOk_WithSystemMetadataAndSupportedServices()
    {
        // Act
        var response = await _client.GetAsync("/api/info");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var info = await response.Content.ReadFromJsonAsync<ApiInfoResponse>();
        Assert.NotNull(info);
        Assert.Equal("OptiTravelAPI", info.Name);
        Assert.Equal("1.0.0", info.Version);
        Assert.Equal(".NET 10", info.Runtime);
        Assert.False(string.IsNullOrWhiteSpace(info.TraceId));

        // Verify all 5 foundational travel services are listed
        Assert.Contains(ServiceTypes.Bus, info.SupportedServices);
        Assert.Contains(ServiceTypes.Hotel, info.SupportedServices);
        Assert.Contains(ServiceTypes.Flight, info.SupportedServices);
        Assert.Contains(ServiceTypes.Cab, info.SupportedServices);
        Assert.Contains(ServiceTypes.Transfers, info.SupportedServices);
    }
}
