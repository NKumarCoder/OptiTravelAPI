using OptiTravelAPI.Web.Extensions;
using OptiTravelAPI.Web.Middleware;

// Register process-level unhandled exception diagnostics
AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
{
    var ex = eventArgs.ExceptionObject as Exception;
    var isTerminating = eventArgs.IsTerminating;
    Console.Error.WriteLine($"[FATAL] AppDomain UnhandledException (IsTerminating: {isTerminating}): {ex?.ToString() ?? eventArgs.ExceptionObject?.ToString()}");
};

TaskScheduler.UnobservedTaskException += (sender, eventArgs) =>
{
    Console.Error.WriteLine($"[ERROR] TaskScheduler UnobservedTaskException: {eventArgs.Exception?.ToString()}");
};

var builder = WebApplication.CreateBuilder(args);

// Configure dynamic port from PORT environment variable (Vercel, Docker, Cloud Run)
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Add controllers
builder.Services.AddControllers();

// Configure Swagger / OpenAPI
builder.Services.AddSwaggerDocumentation(builder.Configuration);

// Register 4-layer architecture & dependencies
builder.Services.AddOptiTravelLayers(builder.Configuration);

var app = builder.Build();

// Application lifetime event logging
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
var appLogger = app.Services.GetRequiredService<ILogger<Program>>();

lifetime.ApplicationStarted.Register(() =>
{
    appLogger.LogInformation("OptiTravelAPI application started successfully on port {Port}.", port);
});

lifetime.ApplicationStopping.Register(() =>
{
    appLogger.LogWarning("OptiTravelAPI application is stopping (graceful shutdown initiated).");
});

lifetime.ApplicationStopped.Register(() =>
{
    appLogger.LogInformation("OptiTravelAPI application has stopped.");
});

// Request tracing & correlation middleware (must be first in pipeline)
app.UseMiddleware<RequestTracingMiddleware>();

// Centralized exception handling middleware
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseRouting();

// Swagger UI (Development or when Swagger:Enabled is true, after UseRouting)
app.UseSwaggerDocumentation(app.Configuration, app.Environment);

app.MapControllers();

// Redirect root to Swagger UI for instant access
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();

// Required for WebApplicationFactory in integration tests
public partial class Program { }
