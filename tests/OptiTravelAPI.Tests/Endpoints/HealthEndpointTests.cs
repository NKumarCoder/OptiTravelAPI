using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using OptiTravelAPI.Shared.Constants;
using OptiTravelAPI.Shared.Models;
using Xunit;

namespace OptiTravelAPI.Tests.Endpoints;

public class HealthEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public HealthEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetHealth_ReturnsOk_WithHealthyStatusAndTraceId()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify TraceId in response header
        Assert.True(response.Headers.Contains(HeaderNames.TraceId));
        var headerTraceId = response.Headers.GetValues(HeaderNames.TraceId).FirstOrDefault();
        Assert.False(string.IsNullOrWhiteSpace(headerTraceId));

        // Verify payload content
        var health = await response.Content.ReadFromJsonAsync<HealthResponse>();
        Assert.NotNull(health);
        Assert.Equal("Healthy", health.Status);
        Assert.Equal("OptiTravelAPI", health.Service);
        Assert.Equal(headerTraceId, health.TraceId);
    }
}
