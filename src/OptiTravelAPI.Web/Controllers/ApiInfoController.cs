using Microsoft.AspNetCore.Mvc;
using OptiTravelAPI.Shared.Constants;
using OptiTravelAPI.Shared.Models;

namespace OptiTravelAPI.Web.Controllers;

/// <summary>
/// General API information endpoint providing metadata about platform version and supported capabilities.
/// </summary>
[ApiController]
[Route("api/info")]
[Produces("application/json")]
public class ApiInfoController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;

    public ApiInfoController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    /// <summary>
    /// Retrieves current API information and supported travel services.
    /// </summary>
    /// <returns>API information response.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiInfoResponse), StatusCodes.Status200OK)]
    public ActionResult<ApiInfoResponse> Get()
    {
        var info = new ApiInfoResponse
        {
            Name = "OptiTravelAPI",
            Version = "1.0.0",
            Description = "Modern Modular Travel Consolidation API Platform",
            Runtime = ".NET 10",
            Environment = _environment.EnvironmentName,
            ServerTimeUtc = DateTime.UtcNow,
            TraceId = HttpContext.TraceIdentifier,
            SupportedServices = ServiceTypes.All
        };

        return Ok(info);
    }
}
