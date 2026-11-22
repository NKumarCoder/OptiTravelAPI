using System.Diagnostics;
using OptiTravelAPI.Shared.Constants;
using OptiTravelAPI.Shared.Utilities;

namespace OptiTravelAPI.Web.Middleware;

/// <summary>
/// Middleware to propagate or generate unique TraceId / CorrelationId for every request,
/// bind it to the logging scope, and measure execution latency.
/// </summary>
public class RequestTracingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestTracingMiddleware> _logger;

    public RequestTracingMiddleware(RequestDelegate next, ILogger<RequestTracingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Check for existing correlation/trace header or create a new one
        string traceId = context.Request.Headers[HeaderNames.CorrelationId].FirstOrDefault()
            ?? context.Request.Headers[HeaderNames.TraceId].FirstOrDefault()
            ?? CorrelationIdGenerator.Generate();

        context.TraceIdentifier = traceId;

        // Set response header so client can trace downstream
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderNames.TraceId] = traceId;
            return Task.CompletedTask;
        });

        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["TraceId"] = traceId,
            ["RequestMethod"] = context.Request.Method,
            ["RequestPath"] = context.Request.Path.Value ?? string.Empty
        }))
        {
            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation("HTTP {Method} {Path} started", context.Request.Method, context.Request.Path);

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("HTTP {Method} {Path} finished with status {StatusCode} in {ElapsedMs:0.00} ms",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.Elapsed.TotalMilliseconds);
            }
        }
    }
}
