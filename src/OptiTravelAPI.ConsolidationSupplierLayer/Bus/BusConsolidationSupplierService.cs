using Microsoft.Extensions.Logging;
using OptiTravelAPI.ConsolidationLayer.Bus;
using OptiTravelAPI.Shared.Models.Bus;

namespace OptiTravelAPI.ConsolidationSupplierLayer.Bus;

/// <summary>
/// Receives requests from the public API layer and forwards them to the Bus consolidation engine.
/// Returns responses without altering supplier structures unnecessarily and handles error propagation.
/// </summary>
public class BusConsolidationSupplierService : IBusConsolidationSupplierService
{
    private readonly IBusConsolidationEngine _busEngine;
    private readonly ILogger<BusConsolidationSupplierService> _logger;

    public BusConsolidationSupplierService(IBusConsolidationEngine busEngine, ILogger<BusConsolidationSupplierService> logger)
    {
        _busEngine = busEngine;
        _logger = logger;
    }

    public async Task<BusLoginResponse> LoginAsync(BusLoginRequest request, string? supplierName = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("ConsolidationSupplierLayer: Login started for supplier '{Supplier}'", supplierName ?? "default");
        var response = await _busEngine.LoginAsync(request, supplierName, cancellationToken);
        _logger.LogInformation("ConsolidationSupplierLayer: Login completed for supplier '{Supplier}'", supplierName ?? "default");
        return response;
    }

    public async Task<BusSearchResponse> SearchAsync(BusSearchRequest request, string? supplierName = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("ConsolidationSupplierLayer: Search started for supplier '{Supplier}'", supplierName ?? "default");
        var response = await _busEngine.SearchAsync(request, supplierName, cancellationToken);
        _logger.LogInformation("ConsolidationSupplierLayer: Search completed for supplier '{Supplier}'", supplierName ?? "default");
        return response;
    }

    public async Task<BusSeatLayoutResponse> SelectSeatsAsync(BusSeatLayoutRequest request, string? supplierName = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("ConsolidationSupplierLayer: SelectSeats started for supplier '{Supplier}'", supplierName ?? "default");
        var response = await _busEngine.SelectSeatsAsync(request, supplierName, cancellationToken);
        _logger.LogInformation("ConsolidationSupplierLayer: SelectSeats completed for supplier '{Supplier}'", supplierName ?? "default");
        return response;
    }

    public async Task<BusBlockResponse> BlockSeatsAsync(BusBlockRequest request, string? supplierName = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("ConsolidationSupplierLayer: BlockSeats started for supplier '{Supplier}'", supplierName ?? "default");
        var response = await _busEngine.BlockSeatsAsync(request, supplierName, cancellationToken);
        _logger.LogInformation("ConsolidationSupplierLayer: BlockSeats completed for supplier '{Supplier}'", supplierName ?? "default");
        return response;
    }

    public async Task<BusBookResponse> BookSeatsAsync(BusBookRequest request, string? supplierName = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("ConsolidationSupplierLayer: BookSeats started for supplier '{Supplier}'", supplierName ?? "default");
        var response = await _busEngine.BookSeatsAsync(request, supplierName, cancellationToken);
        _logger.LogInformation("ConsolidationSupplierLayer: BookSeats completed for supplier '{Supplier}'", supplierName ?? "default");
        return response;
    }

    public async Task<BusTicketResponse> GetTicketAsync(BusTicketRequest request, string? supplierName = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("ConsolidationSupplierLayer: GetTicket started for supplier '{Supplier}'", supplierName ?? "default");
        var response = await _busEngine.GetTicketAsync(request, supplierName, cancellationToken);
        _logger.LogInformation("ConsolidationSupplierLayer: GetTicket completed for supplier '{Supplier}'", supplierName ?? "default");
        return response;
    }

    public async Task<BusTrackingResponse> GetTrackingAsync(BusTrackingRequest request, string? supplierName = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("ConsolidationSupplierLayer: GetTracking started for supplier '{Supplier}'", supplierName ?? "default");
        var response = await _busEngine.GetTrackingAsync(request, supplierName, cancellationToken);
        _logger.LogInformation("ConsolidationSupplierLayer: GetTracking completed for supplier '{Supplier}'", supplierName ?? "default");
        return response;
    }

    public async Task<BusGetTicketDetailsResponse> GetTicketDetailsAsync(string referanceNumber, string? supplierName = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("ConsolidationSupplierLayer: GetTicketDetails started for supplier '{Supplier}'", supplierName ?? "default");
        var response = await _busEngine.GetTicketDetailsAsync(referanceNumber, supplierName, cancellationToken);
        _logger.LogInformation("ConsolidationSupplierLayer: GetTicketDetails completed for supplier '{Supplier}'", supplierName ?? "default");
        return response;
    }

    public async Task<BusPreCancelResponse> PreCancelAsync(BusPreCancelRequest request, string? supplierName = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("ConsolidationSupplierLayer: PreCancel started for supplier '{Supplier}'", supplierName ?? "default");
        var response = await _busEngine.PreCancelAsync(request, supplierName, cancellationToken);
        _logger.LogInformation("ConsolidationSupplierLayer: PreCancel completed for supplier '{Supplier}'", supplierName ?? "default");
        return response;
    }

    public async Task<BusCancelResponse> CancelAsync(BusCancelRequest request, string? supplierName = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("ConsolidationSupplierLayer: Cancel started for supplier '{Supplier}'", supplierName ?? "default");
        var response = await _busEngine.CancelAsync(request, supplierName, cancellationToken);
        _logger.LogInformation("ConsolidationSupplierLayer: Cancel completed for supplier '{Supplier}'", supplierName ?? "default");
        return response;
    }

    public async Task<BusBalanceResponse> GetBalanceAsync(string? supplierName = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("ConsolidationSupplierLayer: GetBalance started for supplier '{Supplier}'", supplierName ?? "default");
        var response = await _busEngine.GetBalanceAsync(supplierName, cancellationToken);
        _logger.LogInformation("ConsolidationSupplierLayer: GetBalance completed for supplier '{Supplier}'", supplierName ?? "default");
        return response;
    }

    public async Task<CdnCitiesResponse> GetCdnCitiesAsync(string? supplierName = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("ConsolidationSupplierLayer: GetCdnCities started for supplier '{Supplier}'", supplierName ?? "default");
        var response = await _busEngine.GetCdnCitiesAsync(supplierName, cancellationToken);
        _logger.LogInformation("ConsolidationSupplierLayer: GetCdnCities completed for supplier '{Supplier}'", supplierName ?? "default");
        return response;
    }
}
