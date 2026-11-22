using Microsoft.AspNetCore.Mvc;
using OptiTravelAPI.ConsolidationSupplierLayer.Bus;
using OptiTravelAPI.Shared.Constants;
using OptiTravelAPI.Shared.Models.Bus;

namespace OptiTravelAPI.Web.Controllers;

/// <summary>
/// CDN Content APIs for static reference data including cities, stops, and operators.
/// </summary>
[ApiController]
[Route("api/cdn")]
[Produces("application/json")]
public class CdnController : ControllerBase
{
    private readonly IBusConsolidationSupplierService _busSupplierService;
    private readonly ILogger<CdnController> _logger;

    public CdnController(
        IBusConsolidationSupplierService busSupplierService,
        ILogger<CdnController> logger)
    {
        _busSupplierService = busSupplierService;
        _logger = logger;
    }

    private string? GetSupplierName() =>
        Request.Headers[HeaderNames.Supplier].FirstOrDefault();

    /// <summary>
    /// Retrieve available cities and station points from CDN.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of cities with names, codes, states, and countries.</returns>
    [HttpGet("cities")]
    [ProducesResponseType(typeof(CdnCitiesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CdnCitiesResponse>> GetCities(CancellationToken cancellationToken)
    {
        var response = await _busSupplierService.GetCdnCitiesAsync(GetSupplierName(), cancellationToken);
        return Ok(response);
    }
}
