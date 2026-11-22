using Microsoft.AspNetCore.Mvc;
using OptiTravelAPI.ConsolidationSupplierLayer.Bus;
using OptiTravelAPI.Shared.Constants;
using OptiTravelAPI.Shared.Models.Bus;

namespace OptiTravelAPI.Web.Controllers;

/// <summary>
/// Public Bus Consolidation APIs supporting Search, Seat Selection, Blocking, Booking, Tracking, and Cancellation.
/// </summary>
[ApiController]
[Route("api/bus")]
[Produces("application/json")]
public class BusController : ControllerBase
{
    private readonly IBusConsolidationSupplierService _busSupplierService;
    private readonly ILogger<BusController> _logger;

    public BusController(
        IBusConsolidationSupplierService busSupplierService,
        ILogger<BusController> logger)
    {
        _busSupplierService = busSupplierService;
        _logger = logger;
    }

    private string? GetSupplierName() =>
        Request.Headers[HeaderNames.Supplier].FirstOrDefault();

    /// <summary>
    /// Authenticate with the supplier and retrieve an access token.
    /// </summary>
    /// <param name="request">Username and password credentials.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Supplier login response with access token.</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(BusLoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BusLoginResponse>> Login(
        [FromBody] BusLoginRequest request,
        CancellationToken cancellationToken)
    {
        var supplier = GetSupplierName() ?? SupplierNames.Tbo;
        _logger.LogInformation("BusController.Login executing for supplier '{Supplier}'", supplier);
        var response = await _busSupplierService.LoginAsync(request, supplier, cancellationToken);
        _logger.LogInformation("BusController.Login completed successfully for supplier '{Supplier}'", supplier);
        return Ok(response);
    }

    /// <summary>
    /// Search Buses across suppliers.
    /// You can get results from multiple trips.
    /// Search by city ("src":"618", "dst":"101") or by GPS ("src":"25.23,42.32", "dst":"45.23,23.41").
    /// When filters is true, you will receive filter aggregations in response.
    /// Tags: Seater, Sleeper, MTicket, etc.
    /// Cancellation Policy Unit: mm = Minutes, HH = Hours, AM = Anti Meridian, PM = Post Meridian.
    /// </summary>
    /// <param name="request">Trip criteria and filter options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Available buses, seat counts, boarding/dropping points, and filters.</returns>
    [HttpPost("search")]
    [ProducesResponseType(typeof(BusSearchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BusSearchResponse>> Search(
        [FromBody] BusSearchRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _busSupplierService.SearchAsync(request, GetSupplierName(), cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Get Seat Layout.
    /// timeD = Departure Time. timeA = Arrival Time.
    /// bpid = Boarding Point Id. dpid = Dropping Point Id.
    /// x, y are points in Quadrant 1.
    /// z: 0 means lower-berth &amp; z: 1 means upper-berth.
    /// AFA = Available For Any, AFM = Available For Male, AFF = Available For Female,
    /// BFA = Booked For Any, BFM = Booked For Male, BFF = Booked For Female.
    /// If bpid + dpid is true, select those to get accurate seat-wise price.
    /// If idproof is true, enter idproof details in BLOCK request.
    /// </summary>
    /// <param name="request">Bus ID, boarding/dropping points, and layout preferences.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Seat layout, berths, availability, and pricing.</returns>
    [HttpPost("select")]
    [ProducesResponseType(typeof(BusSeatLayoutResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BusSeatLayoutResponse>> SelectSeats(
        [FromBody] BusSeatLayoutRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _busSupplierService.SelectSeatsAsync(request, GetSupplierName(), cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Block Seats for a limited duration.
    /// You can block multiple trips with one request.
    /// seatIds: trip-wise seat IDs.
    /// POC = Point of Contact, POI = Proof of Identity.
    /// </summary>
    /// <param name="request">Blocking request containing passenger, POC, and trip details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Blocking ID, reference number, span, and fare updates.</returns>
    [HttpPost("block")]
    [ProducesResponseType(typeof(BusBlockResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BusBlockResponse>> BlockSeats(
        [FromBody] BusBlockRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _busSupplierService.BlockSeatsAsync(request, GetSupplierName(), cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Book Selected Seats using supplier reference numbers.
    /// You can book multiple trips at a time.
    /// </summary>
    /// <param name="request">Booking request with reference numbers.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Ticket number, PNR, ticket key, and operator number.</returns>
    [HttpPost("book")]
    [ProducesResponseType(typeof(BusBookResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BusBookResponse>> BookSeats(
        [FromBody] BusBookRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _busSupplierService.BookSeatsAsync(request, GetSupplierName(), cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Retrieve Ticket Information by ticket number.
    /// </summary>
    /// <param name="request">Ticket number query payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Detailed ticket, passenger, boarding, and bus information.</returns>
    [HttpPost("ticket")]
    [ProducesResponseType(typeof(BusTicketResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BusTicketResponse>> GetTicket(
        [FromBody] BusTicketRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _busSupplierService.GetTicketAsync(request, GetSupplierName(), cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Track Bus Location in real-time.
    /// </summary>
    /// <param name="request">Ticket number query payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vehicle coordinates, driver contact, and live tracking URL.</returns>
    [HttpPost("tracking")]
    [ProducesResponseType(typeof(BusTrackingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BusTrackingResponse>> TrackBus(
        [FromBody] BusTrackingRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _busSupplierService.GetTrackingAsync(request, GetSupplierName(), cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Retrieve Ticket Details by reference number.
    /// </summary>
    /// <param name="referanceNumber">Reference number issued during booking/blocking (exact spelling preserved).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Comprehensive ticket details including passenger list and cancellation policies.</returns>
    [HttpGet("GetTicketDetails")]
    [ProducesResponseType(typeof(BusGetTicketDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BusGetTicketDetailsResponse>> GetTicketDetails(
        [FromQuery] string referanceNumber,
        CancellationToken cancellationToken)
    {
        var response = await _busSupplierService.GetTicketDetailsAsync(referanceNumber, GetSupplierName(), cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Before Cancel Ticket Get Cancellation Information.
    /// NCZ = No Cancellation, YFC = Yes Full Cancellation, YPC = Yes Partial Cancellation, YPF = Yes Full or Partial Cancellation.
    /// </summary>
    /// <param name="request">Ticket number and seats to pre-check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Cancellation refund, charges, and policy notes.</returns>
    [HttpPost("precancel")]
    [ProducesResponseType(typeof(BusPreCancelResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BusPreCancelResponse>> PreCancel(
        [FromBody] BusPreCancelRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _busSupplierService.PreCancelAsync(request, GetSupplierName(), cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Cancel Booked Tickets for selected seats.
    /// </summary>
    /// <param name="request">Ticket number and seats to cancel.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Cancellation status, new ticket ID, and refund breakdown.</returns>
    [HttpPost("cancel")]
    [ProducesResponseType(typeof(BusCancelResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BusCancelResponse>> Cancel(
        [FromBody] BusCancelRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _busSupplierService.CancelAsync(request, GetSupplierName(), cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Get Supplier Account Balance.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Supplier account balance amount and trace ID.</returns>
    [HttpGet("balance")]
    [ProducesResponseType(typeof(BusBalanceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BusBalanceResponse>> GetBalance(CancellationToken cancellationToken)
    {
        var response = await _busSupplierService.GetBalanceAsync(GetSupplierName(), cancellationToken);
        return Ok(response);
    }
}
