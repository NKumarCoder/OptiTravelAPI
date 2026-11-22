using System.Net;
using System.Text.Json;
using OptiTravelAPI.Shared.Exceptions;
using OptiTravelAPI.Shared.Models;

namespace OptiTravelAPI.Web.Middleware;

/// <summary>
/// Centralized global exception handler that catches all unhandled exceptions in the HTTP pipeline,
/// logs structured errors, and returns consistent ErrorResponse payloads without crashing the process.
/// </summary>
public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public GlobalExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;

        // If response headers have already been sent to client, cannot modify status or body
        if (context.Response.HasStarted)
        {
            _logger.LogWarning("Response has already started for [TraceId: {TraceId}], skipping error payload.", traceId);
            return;
        }

        var statusCode = HttpStatusCode.InternalServerError;
        var errorCode = "INTERNAL_SERVER_ERROR";
        var message = "An unexpected error occurred while processing your request.";

        switch (exception)
        {
            case NotFoundException notFoundEx:
                statusCode = HttpStatusCode.NotFound;
                errorCode = notFoundEx.ErrorCode ?? "NOT_FOUND";
                message = notFoundEx.Message;
                _logger.LogWarning(exception, "Resource not found [TraceId: {TraceId}]: {Message}", traceId, message);
                break;

            case SupplierException supplierEx:
                statusCode = supplierEx.UpstreamStatusCode switch
                {
                    400 => HttpStatusCode.BadRequest,
                    401 => HttpStatusCode.Unauthorized,
                    403 => HttpStatusCode.Forbidden,
                    404 => HttpStatusCode.NotFound,
                    504 => HttpStatusCode.GatewayTimeout,
                    _ => HttpStatusCode.BadGateway
                };
                errorCode = supplierEx.ErrorCode ?? "SUPPLIER_COMMUNICATION_ERROR";
                message = _environment.IsDevelopment()
                    ? supplierEx.Message
                    : $"Supplier '{supplierEx.SupplierName}' communication failed.";
                _logger.LogError(exception, "Supplier failure [TraceId: {TraceId}] for supplier {Supplier} in service {Service} (HTTP {UpstreamStatus}): {Message}",
                    traceId, supplierEx.SupplierName, supplierEx.ServiceType, supplierEx.UpstreamStatusCode, supplierEx.Message);
                break;

            case ConsolidationException consolidationEx:
                statusCode = HttpStatusCode.InternalServerError;
                errorCode = consolidationEx.ErrorCode ?? "CONSOLIDATION_ERROR";
                message = _environment.IsDevelopment()
                    ? consolidationEx.Message
                    : $"Consolidation failed for service '{consolidationEx.ServiceType}'.";
                _logger.LogError(exception, "Consolidation error [TraceId: {TraceId}] for service {Service}: {Message}",
                    traceId, consolidationEx.ServiceType, consolidationEx.Message);
                break;

            case OperationCanceledException:
                if (context.RequestAborted.IsCancellationRequested)
                {
                    statusCode = (HttpStatusCode)499; // Client Closed Request
                    errorCode = "REQUEST_CANCELLED";
                    message = "The operation was cancelled by the client.";
                    _logger.LogInformation("Request cancelled by client [TraceId: {TraceId}]", traceId);
                }
                else
                {
                    statusCode = HttpStatusCode.GatewayTimeout;
                    errorCode = "GATEWAY_TIMEOUT";
                    message = "The operation timed out.";
                    _logger.LogError(exception, "Operation timed out [TraceId: {TraceId}]", traceId);
                }
                break;

            case TimeoutException timeoutEx:
                statusCode = HttpStatusCode.GatewayTimeout;
                errorCode = "GATEWAY_TIMEOUT";
                message = _environment.IsDevelopment() ? timeoutEx.Message : "The operation timed out.";
                _logger.LogError(exception, "Timeout occurred [TraceId: {TraceId}]: {Message}", traceId, timeoutEx.Message);
                break;

            case HttpRequestException httpEx:
                statusCode = httpEx.StatusCode ?? HttpStatusCode.BadGateway;
                errorCode = "UPSTREAM_NETWORK_ERROR";
                message = _environment.IsDevelopment() ? httpEx.Message : "A network error occurred while contacting upstream service.";
                _logger.LogError(exception, "HTTP request network failure [TraceId: {TraceId}]: {Message}", traceId, httpEx.Message);
                break;

            case ArgumentException argEx:
                statusCode = HttpStatusCode.BadRequest;
                errorCode = "INVALID_ARGUMENT";
                message = argEx.Message;
                _logger.LogWarning(exception, "Invalid argument [TraceId: {TraceId}]: {Message}", traceId, argEx.Message);
                break;

            case OptiTravelException domainEx:
                statusCode = HttpStatusCode.BadRequest;
                errorCode = domainEx.ErrorCode ?? "APPLICATION_ERROR";
                message = domainEx.Message;
                _logger.LogWarning(exception, "Application domain error [TraceId: {TraceId}]: {Message}", traceId, message);
                break;

            default:
                _logger.LogError(exception, "Unhandled exception [TraceId: {TraceId}]: {Message}", traceId, exception.Message);
                if (_environment.IsDevelopment())
                {
                    message = exception.Message;
                }
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var errorResponse = new ErrorResponse
        {
            TraceId = traceId,
            StatusCode = (int)statusCode,
            ErrorCode = errorCode,
            Message = message,
            TimestampUtc = DateTime.UtcNow
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, SerializerOptions));
    }
}
