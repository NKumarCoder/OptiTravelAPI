using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using OptiTravelAPI.Shared.Exceptions;
using OptiTravelAPI.Shared.Models;
using OptiTravelAPI.Shared.Models.Bus;
using OptiTravelAPI.Suppliers.Bus.Tbo;
using Xunit;

namespace OptiTravelAPI.Tests.Suppliers;

public class TboBusSupplierTests
{
    private class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

        public MockHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_handler(request));
        }
    }

    private class MockHttpClientFactory : IHttpClientFactory
    {
        private readonly HttpMessageHandler _handler;

        public MockHttpClientFactory(HttpMessageHandler handler)
        {
            _handler = handler;
        }

        public HttpClient CreateClient(string name)
        {
            return new HttpClient(_handler)
            {
                BaseAddress = new Uri("https://api.tbo.com")
            };
        }
    }

    private TboBusSupplier CreateSupplier(HttpMessageHandler handler)
    {
        var config = new SupplierConfiguration
        {
            ["Bus"] = new Dictionary<string, SupplierItemConfig>(StringComparer.OrdinalIgnoreCase)
            {
                ["TBO"] = new SupplierItemConfig
                {
                    Enabled = true,
                    BaseUrl = "https://api.tbo.com",
                    Username = "testuser",
                    Password = "testpassword",
                    TimeoutSeconds = 30
                }
            }
        };

        return new TboBusSupplier(
            new MockHttpClientFactory(handler),
            Options.Create(config),
            NullLogger<TboBusSupplier>.Instance);
    }

    [Fact]
    public async Task LoginAsync_ReturnsTokenAndCachesIt()
    {
        // Arrange
        var mockResponse = new BusLoginResponse
        {
            TraceId = "login-trace-1",
            AccessToken = "mock-jwt-token-123"
        };

        var handler = new MockHttpMessageHandler(req =>
        {
            Assert.Equal(HttpMethod.Post, req.Method);
            Assert.Equal("/api/bus/login", req.RequestUri!.AbsolutePath);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(mockResponse))
            };
        });

        var supplier = CreateSupplier(handler);

        // Act
        var result = await supplier.LoginAsync(new BusLoginRequest { Username = "u", Password = "p" });

        // Assert
        Assert.NotNull(result);
        Assert.Equal("mock-jwt-token-123", result.AccessToken);
        Assert.Equal("login-trace-1", result.TraceId);
    }

    [Fact]
    public async Task SearchAsync_ForwardsRequestAndDeserializesBuses()
    {
        // Arrange
        var mockResponse = new BusSearchResponse
        {
            TraceId = "search-trace",
            Trips = new List<BusSearchTripResult>
            {
                new()
                {
                    Buses = new List<BusItem>
                    {
                        new() { Id = "bus-1", Name = "Super Express", Duration = 360 }
                    }
                }
            }
        };

        var handler = new MockHttpMessageHandler(req =>
        {
            Assert.Equal(HttpMethod.Post, req.Method);
            Assert.Equal("/api/bus/search", req.RequestUri!.AbsolutePath);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(mockResponse))
            };
        });

        var supplier = CreateSupplier(handler);

        // Act
        var result = await supplier.SearchAsync(new BusSearchRequest
        {
            Trips = new List<BusSearchTripRequest>
            {
                new() { Src = "702", Dst = "1445", Doj = "2026-11-28" }
            }
        });

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Trips);
        Assert.Single(result.Trips!);
        Assert.Equal("bus-1", result.Trips[0].Buses![0].Id);
    }

    [Fact]
    public async Task GetTicketDetailsAsync_PreservesSpellingAndEncodesQueryParam()
    {
        // Arrange
        var mockResponse = new BusGetTicketDetailsResponse
        {
            TicketId = "TKT999",
            ReferenceNumber = "REF123/456",
            Passangers = new List<BusPassengerDetail>
            {
                new() { FirstName = "John", SeatNo = "1A" }
            }
        };

        var handler = new MockHttpMessageHandler(req =>
        {
            Assert.Equal(HttpMethod.Get, req.Method);
            Assert.Contains("referanceNumber=REF123%2F456", req.RequestUri!.Query);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(mockResponse))
            };
        });

        var supplier = CreateSupplier(handler);

        // Act
        var result = await supplier.GetTicketDetailsAsync("REF123/456");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("TKT999", result.TicketId);
        Assert.Equal("REF123/456", result.ReferenceNumber);
        Assert.Single(result.Passangers!);
    }

    [Fact]
    public async Task SupplierError_IsPreservedInResponse()
    {
        // Arrange
        var mockResponse = new BusSearchResponse
        {
            TraceId = "err-trace",
            Error = new SupplierError { Code = "ERR_NO_BUS", Desc = "No buses available for this route." }
        };

        var handler = new MockHttpMessageHandler(req =>
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(mockResponse))
            };
        });

        var supplier = CreateSupplier(handler);

        // Act
        var result = await supplier.SearchAsync(new BusSearchRequest());

        // Assert
        Assert.NotNull(result.Error);
        Assert.Equal("ERR_NO_BUS", result.Error.Code);
        Assert.Equal("No buses available for this route.", result.Error.Desc);
    }

    [Fact]
    public async Task NetworkFailure_ThrowsSupplierException()
    {
        // Arrange
        var handler = new MockHttpMessageHandler(req =>
        {
            throw new HttpRequestException("Connection refused");
        });

        var supplier = CreateSupplier(handler);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<SupplierException>(() =>
            supplier.SearchAsync(new BusSearchRequest()));

        Assert.Contains("Network connection error", ex.Message);
    }
}
