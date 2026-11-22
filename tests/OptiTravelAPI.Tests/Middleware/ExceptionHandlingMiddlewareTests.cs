using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using OptiTravelAPI.Shared.Exceptions;
using OptiTravelAPI.Shared.Models;
using OptiTravelAPI.Web.Middleware;
using Xunit;

namespace OptiTravelAPI.Tests.Middleware;

public class ExceptionHandlingMiddlewareTests
{
    private class FakeHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = Environments.Production;
        public string ApplicationName { get; set; } = "OptiTravelAPI";
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; } = null!;
    }

    [Fact]
    public async Task InvokeAsync_CatchesNotFoundException_Returns404WithStructuredResponse()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.TraceIdentifier = "test-trace-123";

        var middleware = new GlobalExceptionHandlingMiddleware(
            _ => throw new NotFoundException("Item was not found", "ITEM_NOT_FOUND"),
            NullLogger<GlobalExceptionHandlingMiddleware>.Instance,
            new FakeHostEnvironment()
        );

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal((int)HttpStatusCode.NotFound, context.Response.StatusCode);
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var error = JsonSerializer.Deserialize<ErrorResponse>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(error);
        Assert.Equal("test-trace-123", error.TraceId);
        Assert.Equal((int)HttpStatusCode.NotFound, error.StatusCode);
        Assert.Equal("ITEM_NOT_FOUND", error.ErrorCode);
        Assert.Equal("Item was not found", error.Message);
    }

    [Fact]
    public async Task InvokeAsync_CatchesSupplierException_ReturnsCorrectStatusAndDetails()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.TraceIdentifier = "supplier-trace-456";

        var middleware = new GlobalExceptionHandlingMiddleware(
            _ => throw new SupplierException("TBO", "Bus", "Supplier network failure", upstreamStatusCode: null),
            NullLogger<GlobalExceptionHandlingMiddleware>.Instance,
            new FakeHostEnvironment()
        );

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal((int)HttpStatusCode.BadGateway, context.Response.StatusCode);
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var error = JsonSerializer.Deserialize<ErrorResponse>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(error);
        Assert.Equal("supplier-trace-456", error.TraceId);
        Assert.Equal((int)HttpStatusCode.BadGateway, error.StatusCode);
        Assert.Equal("SUPPLIER_COMMUNICATION_ERROR", error.ErrorCode);
    }

    [Fact]
    public async Task InvokeAsync_CatchesSupplierTimeout_Returns504GatewayTimeout()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.TraceIdentifier = "timeout-trace-789";

        var middleware = new GlobalExceptionHandlingMiddleware(
            _ => throw new SupplierException("TBO", "Bus", "Upstream timed out", 504, "TIMEOUT"),
            NullLogger<GlobalExceptionHandlingMiddleware>.Instance,
            new FakeHostEnvironment()
        );

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal((int)HttpStatusCode.GatewayTimeout, context.Response.StatusCode);
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var error = JsonSerializer.Deserialize<ErrorResponse>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(error);
        Assert.Equal("timeout-trace-789", error.TraceId);
        Assert.Equal(504, error.StatusCode);
        Assert.Equal("TIMEOUT", error.ErrorCode);
    }

    [Fact]
    public async Task InvokeAsync_CatchesClientCancellation_Returns499ClientClosedRequest()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var context = new DefaultHttpContext();
        context.RequestAborted = cts.Token;
        context.Response.Body = new MemoryStream();
        context.TraceIdentifier = "cancelled-trace-999";

        var middleware = new GlobalExceptionHandlingMiddleware(
            _ => throw new OperationCanceledException(cts.Token),
            NullLogger<GlobalExceptionHandlingMiddleware>.Instance,
            new FakeHostEnvironment()
        );

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(499, context.Response.StatusCode);
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var error = JsonSerializer.Deserialize<ErrorResponse>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(error);
        Assert.Equal("cancelled-trace-999", error.TraceId);
        Assert.Equal(499, error.StatusCode);
        Assert.Equal("REQUEST_CANCELLED", error.ErrorCode);
    }
}
