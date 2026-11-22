using Microsoft.AspNetCore.Mvc;
using OptiTravelAPI.Shared.Models;

namespace OptiTravelAPI.Web.Controllers;

/// <summary>
/// Health check endpoint for system monitoring, load balancers, and container orchestration.
/// </summary>
[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class HealthController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;

    public HealthController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    /// <summary>
    /// Returns current health status of the API.
    /// </summary>
    /// <returns>Health status details.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(HealthResponse), StatusCodes.Status200OK)]
    public ActionResult<HealthResponse> Get()
    {
        var response = new HealthResponse
        {
            Status = "Healthy",
            Service = "OptiTravelAPI",
            Environment = _environment.EnvironmentName,
            TimestampUtc = DateTime.UtcNow,
            TraceId = HttpContext.TraceIdentifier
        };

        return Ok(response);
    }
}
