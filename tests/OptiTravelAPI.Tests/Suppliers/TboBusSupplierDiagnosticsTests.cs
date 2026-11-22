using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using OptiTravelAPI.Shared.Exceptions;
using OptiTravelAPI.Shared.Models;
using OptiTravelAPI.Shared.Models.Bus;
using OptiTravelAPI.Suppliers.Bus.Tbo;
using OptiTravelAPI.Web.Middleware;
using Xunit;

namespace OptiTravelAPI.Tests.Suppliers;

public class TboBusSupplierDiagnosticsTests
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
            return new HttpClient(_handler);
        }
    }

    private TboBusSupplier CreateSupplier(HttpMessageHandler handler, int timeoutSeconds = 30)
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
                    TimeoutSeconds = timeoutSeconds
                }
            }
        };

        return new TboBusSupplier(
            new MockHttpClientFactory(handler),
            Options.Create(config),
            NullLogger<TboBusSupplier>.Instance);
    }

    // 1. Normal supplier response does not terminate application
    [Fact]
    public async Task Scenario1_NormalSupplierResponse_SucceedsWithoutTerminating()
    {
        var mockResponse = new BusLoginResponse
        {
            TraceId = "trace-normal-1",
            AccessToken = "valid-token-xyz"
        };

        var handler = new MockHttpMessageHandler(req =>
        {
            Assert.Equal(HttpMethod.Post, req.Method);
            Assert.Equal("https://api.tbo.com/api/bus/login", req.RequestUri!.ToString());
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(mockResponse))
            };
        });

        var supplier = CreateSupplier(handler);
        var result = await supplier.LoginAsync(new BusLoginRequest { Username = "u", Password = "p" });

        Assert.NotNull(result);
        Assert.Equal("valid-token-xyz", result.AccessToken);
    }

    // 2. A supplier HTTP 400 response is handled correctly
    [Fact]
    public async Task Scenario2_SupplierHttp400Response_ThrowsSupplierExceptionWithBadRequest()
    {
        var errorContent = JsonSerializer.Serialize(new
        {
            error = new { code = "INVALID_PAYLOAD", desc = "Invalid origin city ID" }
        });

        var handler = new MockHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent(errorContent)
        });

        var supplier = CreateSupplier(handler);
        var ex = await Assert.ThrowsAsync<SupplierException>(() =>
            supplier.LoginAsync(new BusLoginRequest { Username = "u", Password = "p" }));

        Assert.Equal(400, ex.UpstreamStatusCode);
        Assert.Equal("INVALID_PAYLOAD", ex.ErrorCode);
        Assert.Contains("Invalid origin city ID", ex.Message);
    }

    // 3. A supplier HTTP 401 response is handled correctly
    [Fact]
    public async Task Scenario3_SupplierHttp401Response_ThrowsSupplierExceptionWithUnauthorized()
    {
        var errorContent = JsonSerializer.Serialize(new
        {
            error = new { code = "AUTH_FAILED", desc = "Invalid username or password" }
        });

        var handler = new MockHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.Unauthorized)
        {
            Content = new StringContent(errorContent)
        });

        var supplier = CreateSupplier(handler);
        var ex = await Assert.ThrowsAsync<SupplierException>(() =>
            supplier.LoginAsync(new BusLoginRequest { Username = "u", Password = "p" }));

        Assert.Equal(401, ex.UpstreamStatusCode);
        Assert.Equal("AUTH_FAILED", ex.ErrorCode);
    }

    // 4. A supplier HTTP 500 response is handled correctly
    [Fact]
    public async Task Scenario4_SupplierHttp500Response_ThrowsSupplierExceptionWithUpstreamServerError()
    {
        var handler = new MockHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("Internal Server Error in upstream cluster")
        });

        var supplier = CreateSupplier(handler);
        var ex = await Assert.ThrowsAsync<SupplierException>(() =>
            supplier.LoginAsync(new BusLoginRequest { Username = "u", Password = "p" }));

        Assert.Equal(500, ex.UpstreamStatusCode);
        Assert.Equal("UPSTREAM_SERVER_ERROR", ex.ErrorCode);
        Assert.NotNull(ex.RawResponseBody);
    }

    // 5. A supplier timeout is handled correctly
    [Fact]
    public async Task Scenario5_SupplierTimeout_ThrowsSupplierExceptionWithTimeoutCode()
    {
        var handler = new MockHttpMessageHandler(_ =>
        {
            // Simulate timeout by throwing TaskCanceledException without client cancellation
            throw new TaskCanceledException("The operation was canceled due to timeout.");
        });

        var supplier = CreateSupplier(handler);
        var ex = await Assert.ThrowsAsync<SupplierException>(() =>
            supplier.LoginAsync(new BusLoginRequest { Username = "u", Password = "p" }));

        Assert.Equal(504, ex.UpstreamStatusCode);
        Assert.Equal("TIMEOUT", ex.ErrorCode);
    }

    // 6. A network exception is handled correctly
    [Fact]
    public async Task Scenario6_NetworkException_ThrowsSupplierExceptionWithNetworkErrorCode()
    {
        var handler = new MockHttpMessageHandler(_ =>
        {
            throw new HttpRequestException("Connection refused by remote host.", null, HttpStatusCode.BadGateway);
        });

        var supplier = CreateSupplier(handler);
        var ex = await Assert.ThrowsAsync<SupplierException>(() =>
            supplier.LoginAsync(new BusLoginRequest { Username = "u", Password = "p" }));

        Assert.Equal(502, ex.UpstreamStatusCode);
        Assert.Equal("NETWORK_ERROR", ex.ErrorCode);
        Assert.Contains("Connection refused", ex.Message);
    }

    // 7. An unexpected controller exception returns a structured error response
    [Fact]
    public async Task Scenario7_UnexpectedExceptionInPipeline_ReturnsStructuredErrorResponse()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.TraceIdentifier = "structured-trace-777";

        var middleware = new GlobalExceptionHandlingMiddleware(
            _ => throw new InvalidOperationException("Something unexpected failed internally"),
            NullLogger<GlobalExceptionHandlingMiddleware>.Instance,
            new FakeHostEnv()
        );

        await middleware.InvokeAsync(context);

        Assert.Equal(500, context.Response.StatusCode);
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var error = JsonSerializer.Deserialize<ErrorResponse>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(error);
        Assert.Equal("structured-trace-777", error.TraceId);
        Assert.Equal(500, error.StatusCode);
        Assert.Equal("INTERNAL_SERVER_ERROR", error.ErrorCode);
    }

    // 8. TraceId is preserved in middleware response
    [Fact]
    public async Task Scenario8_TraceId_IsPreservedInAllErrorResponses()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.TraceIdentifier = "preserve-trace-8888";

        var middleware = new GlobalExceptionHandlingMiddleware(
            _ => throw new SupplierException("TBO", "Bus", "Upstream server failure", 500, "UPSTREAM_SERVER_ERROR"),
            NullLogger<GlobalExceptionHandlingMiddleware>.Instance,
            new FakeHostEnv()
        );

        await middleware.InvokeAsync(context);

        Assert.Equal(502, context.Response.StatusCode);
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var error = JsonSerializer.Deserialize<ErrorResponse>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(error);
        Assert.Equal("preserve-trace-8888", error.TraceId);
    }

    // 9. A subsequent request succeeds after a failed supplier request
    [Fact]
    public async Task Scenario9_SubsequentRequestSucceeds_AfterFailedSupplierRequest()
    {
        var callCount = 0;
        var handler = new MockHttpMessageHandler(_ =>
        {
            callCount++;
            if (callCount == 1)
            {
                // First call fails with 500
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("Transient server failure")
                };
            }

            // Second call succeeds
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new BusLoginResponse
                {
                    TraceId = "subsequent-trace-ok",
                    AccessToken = "token-recovered"
                }))
            };
        });

        var supplier = CreateSupplier(handler);

        // 1st request fails
        await Assert.ThrowsAsync<SupplierException>(() =>
            supplier.LoginAsync(new BusLoginRequest { Username = "u", Password = "p" }));

        // 2nd request succeeds without application dying
        var result = await supplier.LoginAsync(new BusLoginRequest { Username = "u", Password = "p" });
        Assert.NotNull(result);
        Assert.Equal("token-recovered", result.AccessToken);
    }

    private class FakeHostEnv : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = Environments.Production;
        public string ApplicationName { get; set; } = "OptiTravelAPI";
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; } = null!;
    }
}
