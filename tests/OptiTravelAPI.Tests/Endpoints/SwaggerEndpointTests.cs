using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace OptiTravelAPI.Tests.Endpoints;

public class SwaggerEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public SwaggerEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetSwaggerJson_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/swagger/v1/swagger.json");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("OptiTravelAPI", content);
    }

    [Fact]
    public async Task GetSwaggerUI_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/swagger/index.html");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("swagger-ui", content);
    }

    [Fact]
    public async Task GetRoot_RedirectsToSwagger()
    {
        // Act
        var clientWithoutRedirect = _client;
        var response = await clientWithoutRedirect.GetAsync("/");

        // By default WebApplicationFactory client follows redirects, so it resolves to /swagger -> /swagger/index.html
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("swagger-ui", content);
    }

    [Fact]
    public async Task GetSwaggerWithoutSlash_RedirectsOrLoadsSwaggerUI()
    {
        // Act
        var response = await _client.GetAsync("/swagger");

        // WebApplicationFactory follows redirects to /swagger/index.html
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("swagger-ui", content);
    }
}
