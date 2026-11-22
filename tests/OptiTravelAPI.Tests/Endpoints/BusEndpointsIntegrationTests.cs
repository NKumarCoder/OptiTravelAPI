using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using OptiTravelAPI.Shared.Constants;
using OptiTravelAPI.Shared.Models.Bus;
using OptiTravelAPI.SupplierLayer.Abstractions;
using OptiTravelAPI.SupplierLayer.Bus;
using OptiTravelAPI.Suppliers.Bus.Tbo;
using Xunit;

namespace OptiTravelAPI.Tests.Endpoints;

public class BusEndpointsIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    private class MockBusSupplier : IBusSupplier
    {
        public string SupplierName => SupplierNames.Tbo;
        public string ServiceType => ServiceTypes.Bus;
        public bool IsEnabled => true;

        public Task<BusLoginResponse> LoginAsync(BusLoginRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new BusLoginResponse { TraceId = "trace-login", AccessToken = "test-token" });

        public Task<BusSearchResponse> SearchAsync(BusSearchRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new BusSearchResponse
            {
                TraceId = "trace-search",
                Trips = new List<BusSearchTripResult>
                {
                    new()
                    {
                        Buses = new List<BusItem>
                        {
                            new() { Id = "bus-101", Name = "Test Travels", Duration = 480 }
                        }
                    }
                }
            });

        public Task<BusSeatLayoutResponse> SelectSeatsAsync(BusSeatLayoutRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new BusSeatLayoutResponse
            {
                TraceId = "trace-select",
                TripKey = "test-trip-key",
                Seats = new List<BusSeatItem>
                {
                    new() { Id = "1A", Name = "1A", Status = "AFA", X = 1, Y = 1, Z = 0 }
                }
            });

        public Task<BusBlockResponse> BlockSeatsAsync(BusBlockRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new BusBlockResponse
            {
                TraceId = "trace-block",
                Trips = new List<BusBlockTripResult>
                {
                    new() { BlockingId = "BLK123", RefNo = "REF456", Status = "BLOCKED" }
                }
            });

        public Task<BusBookResponse> BookSeatsAsync(BusBookRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new BusBookResponse
            {
                TraceId = "trace-book",
                Trips = new List<BusBookTripResult>
                {
                    new() { RefNo = "REF456", TktNo = "TKT789", PnrNo = "PNR999", Status = "CONFIRMED" }
                }
            });

        public Task<BusTicketResponse> GetTicketAsync(BusTicketRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new BusTicketResponse
            {
                TraceId = "trace-ticket",
                TicketNo = request.TktNo,
                Pnr = "PNR999",
                Status = "CONFIRMED"
            });

        public Task<BusTrackingResponse> GetTrackingAsync(BusTrackingRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new BusTrackingResponse
            {
                TraceId = "trace-tracking",
                Tracking = new BusTrackingDetails
                {
                    Status = "Active",
                    Vehicle = new BusVehicleInfo { Number = "KA-01-AB-1234" },
                    Location = new BusLocationInfo { Lat = 12.9716, Lon = 77.5946, Speed = 65 }
                }
            });

        public Task<BusGetTicketDetailsResponse> GetTicketDetailsAsync(string referanceNumber, CancellationToken cancellationToken = default) =>
            Task.FromResult(new BusGetTicketDetailsResponse
            {
                TraceId = "trace-details",
                ReferenceNumber = referanceNumber,
                TicketId = "TKT789",
                Passangers = new List<BusPassengerDetail>
                {
                    new() { FirstName = "Jane", SeatNo = "1A", Age = 28 }
                }
            });

        public Task<BusPreCancelResponse> PreCancelAsync(BusPreCancelRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new BusPreCancelResponse
            {
                TraceId = "trace-precancel",
                Tickets = new List<BusPreCancelTicketItem>
                {
                    new() { Type = "YFC", TotalFare = 1000, Refund = 900, Charge = 100 }
                }
            });

        public Task<BusCancelResponse> CancelAsync(BusCancelRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new BusCancelResponse
            {
                TraceId = "trace-cancel",
                Status = "CANCELLED",
                Fares = new BusCancelFareInfo { Refund = 900, Charge = 100 }
            });

        public Task<BusBalanceResponse> GetBalanceAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(new BusBalanceResponse
            {
                TraceId = "trace-balance",
                Amount = 50000.50m
            });

        public Task<CdnCitiesResponse> GetCdnCitiesAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(new CdnCitiesResponse
            {
                Cities = new List<CityItem>
                {
                    new() { Code = "702", Name = "Bangalore", State = "Karnataka", Country = "India" }
                }
            });
    }

    public BusEndpointsIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove existing suppliers and register MockBusSupplier
                var toRemove = services.Where(d =>
                    d.ServiceType == typeof(IBusSupplier) ||
                    d.ServiceType == typeof(ISupplier) ||
                    d.ServiceType == typeof(TboBusSupplier)).ToList();

                foreach (var descriptor in toRemove)
                {
                    services.Remove(descriptor);
                }

                services.AddScoped<MockBusSupplier>();
                services.AddScoped<IBusSupplier>(sp => sp.GetRequiredService<MockBusSupplier>());
                services.AddScoped<ISupplier>(sp => sp.GetRequiredService<MockBusSupplier>());
            });
        });
    }

    [Fact]
    public async Task PostLogin_ReturnsSuccess_WithToken()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/bus/login", new BusLoginRequest
        {
            Username = "user",
            Password = "userword"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<BusLoginResponse>();
        Assert.NotNull(body);
        Assert.Equal("test-token", body.AccessToken);
    }

    [Fact]
    public async Task PostSearch_ReturnsSuccess_WithBuses()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/bus/search", new BusSearchRequest
        {
            Trips = new List<BusSearchTripRequest>
            {
                new() { Src = "702", Dst = "1445", Doj = "2026-11-28" }
            }
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<BusSearchResponse>();
        Assert.NotNull(body?.Trips);
        Assert.Single(body.Trips);
        Assert.Equal("bus-101", body.Trips[0].Buses![0].Id);
    }

    [Fact]
    public async Task PostSelect_ReturnsSuccess_WithSeatLayout()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/bus/select", new BusSeatLayoutRequest
        {
            BusId = "1#12345"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<BusSeatLayoutResponse>();
        Assert.NotNull(body?.Seats);
        Assert.Single(body.Seats);
        Assert.Equal("1A", body.Seats[0].Id);
    }

    [Fact]
    public async Task PostBlock_ReturnsSuccess_WithBlockingId()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/bus/block", new BusBlockRequest
        {
            Trips = new List<BusBlockTripItem>
            {
                new() { TripKey = "TK1", Bpid = "BP1", Dpid = "DP1" }
            },
            Poc = new BusPoc { Email = "test@test.com", Phone = "9999999999" },
            Pax = new List<BusPassengerBlock>
            {
                new() { Name = "John", Age = 30, Gender = "M", SeatIds = new List<string> { "1A" } }
            }
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<BusBlockResponse>();
        Assert.NotNull(body?.Trips);
        Assert.Equal("BLK123", body.Trips[0].BlockingId);
    }

    [Fact]
    public async Task PostBook_ReturnsSuccess_WithTicketNo()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/bus/book", new BusBookRequest
        {
            Trips = new List<BusBookTripItem>
            {
                new() { RefNo = "REF456" }
            }
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<BusBookResponse>();
        Assert.NotNull(body?.Trips);
        Assert.Equal("TKT789", body.Trips[0].TktNo);
    }

    [Fact]
    public async Task PostTicket_ReturnsSuccess_WithTicketDetails()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/bus/ticket", new BusTicketRequest
        {
            TktNo = "TKT789"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<BusTicketResponse>();
        Assert.NotNull(body);
        Assert.Equal("TKT789", body.TicketNo);
    }

    [Fact]
    public async Task PostTracking_ReturnsSuccess_WithCoordinates()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/bus/tracking", new BusTrackingRequest
        {
            TktNo = "TKT789"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<BusTrackingResponse>();
        Assert.NotNull(body?.Tracking);
        Assert.Equal("KA-01-AB-1234", body.Tracking.Vehicle?.Number);
    }

    [Fact]
    public async Task GetTicketDetails_ReturnsSuccess_WithSpelledParam()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/bus/GetTicketDetails?referanceNumber=REF456");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<BusGetTicketDetailsResponse>();
        Assert.NotNull(body);
        Assert.Equal("REF456", body.ReferenceNumber);
        Assert.Single(body.Passangers!);
    }

    [Fact]
    public async Task PostPreCancel_ReturnsSuccess_WithRefundInfo()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/bus/precancel", new BusPreCancelRequest
        {
            TktNo = "TKT789",
            Seats = new List<string> { "1A" }
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<BusPreCancelResponse>();
        Assert.NotNull(body?.Tickets);
        Assert.Equal(900, body.Tickets[0].Refund);
    }

    [Fact]
    public async Task PostCancel_ReturnsSuccess_WithCancelledStatus()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/bus/cancel", new BusCancelRequest
        {
            TktNo = "TKT789",
            Seats = new List<string> { "1A" }
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<BusCancelResponse>();
        Assert.NotNull(body);
        Assert.Equal("CANCELLED", body.Status);
    }

    [Fact]
    public async Task GetBalance_ReturnsSuccess_WithAmount()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/bus/balance");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<BusBalanceResponse>();
        Assert.NotNull(body);
        Assert.Equal(50000.50m, body.Amount);
    }

    [Fact]
    public async Task GetCdnCities_ReturnsSuccess_WithCityList()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/cdn/cities");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<CdnCitiesResponse>();
        Assert.NotNull(body?.Cities);
        Assert.Single(body.Cities);
        Assert.Equal("Bangalore", body.Cities[0].Name);
    }

    [Fact]
    public async Task CustomSupplierHeader_PropagatesCorrectly()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(HeaderNames.Supplier, "TBO");
        var response = await client.GetAsync("/api/bus/balance");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.Contains(HeaderNames.TraceId));
    }
}
