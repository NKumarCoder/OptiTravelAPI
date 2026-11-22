using Microsoft.Extensions.Logging;
using OptiTravelAPI.Shared.Constants;
using OptiTravelAPI.Shared.Models.Bus;
using OptiTravelAPI.SupplierLayer.Abstractions;
using OptiTravelAPI.SupplierLayer.Bus;

namespace OptiTravelAPI.ConsolidationLayer.Bus;

/// <summary>
/// Implementation of Bus consolidation engine.
/// Responsible for supplier selection, multi-supplier orchestration, and result aggregation.
/// </summary>
public class BusConsolidationEngine : IBusConsolidationEngine
{
    private readonly ISupplierResolver _supplierResolver;
    private readonly ILogger<BusConsolidationEngine> _logger;

    public BusConsolidationEngine(ISupplierResolver supplierResolver, ILogger<BusConsolidationEngine> logger)
    {
        _supplierResolver = supplierResolver;
        _logger = logger;
    }

    private IBusSupplier ResolveSupplier(string? supplierName)
    {
        var targetSupplier = string.IsNullOrWhiteSpace(supplierName) ? SupplierNames.Tbo : supplierName;
        _logger.LogInformation("ConsolidationLayer: Resolving supplier '{Supplier}'", targetSupplier);
        return _supplierResolver.GetSupplier<IBusSupplier>(targetSupplier);
    }

    public async Task<BusLoginResponse> LoginAsync(BusLoginRequest request, string? supplierName = null, CancellationToken cancellationToken = default)
    {
        var supplier = ResolveSupplier(supplierName);
        _logger.LogInformation("ConsolidationLayer: Forwarding Login to SupplierLayer for '{Supplier}'", supplier.SupplierName);
        var response = await supplier.LoginAsync(request, cancellationToken);
        _logger.LogInformation("ConsolidationLayer: Received Login response from '{Supplier}'", supplier.SupplierName);
        return response;
    }

    public async Task<BusSearchResponse> SearchAsync(BusSearchRequest request, string? supplierName = null, CancellationToken cancellationToken = default)
    {
        var supplier = ResolveSupplier(supplierName);
        _logger.LogInformation("ConsolidationLayer: Forwarding Search to SupplierLayer for '{Supplier}'", supplier.SupplierName);
        var response = await supplier.SearchAsync(request, cancellationToken);
        _logger.LogInformation("ConsolidationLayer: Received Search response from '{Supplier}'", supplier.SupplierName);
        return response;
    }

    public async Task<BusSeatLayoutResponse> SelectSeatsAsync(BusSeatLayoutRequest request, string? supplierName = null, CancellationToken cancellationToken = default)
    {
        var supplier = ResolveSupplier(supplierName);
        _logger.LogInformation("ConsolidationLayer: Forwarding SelectSeats to SupplierLayer for '{Supplier}'", supplier.SupplierName);
        var response = await supplier.SelectSeatsAsync(request, cancellationToken);
        _logger.LogInformation("ConsolidationLayer: Received SelectSeats response from '{Supplier}'", supplier.SupplierName);
        return response;
    }

    public async Task<BusBlockResponse> BlockSeatsAsync(BusBlockRequest request, string? supplierName = null, CancellationToken cancellationToken = default)
    {
        var supplier = ResolveSupplier(supplierName);
        _logger.LogInformation("ConsolidationLayer: Forwarding BlockSeats to SupplierLayer for '{Supplier}'", supplier.SupplierName);
        var response = await supplier.BlockSeatsAsync(request, cancellationToken);
        _logger.LogInformation("ConsolidationLayer: Received BlockSeats response from '{Supplier}'", supplier.SupplierName);
        return response;
    }

    public async Task<BusBookResponse> BookSeatsAsync(BusBookRequest request, string? supplierName = null, CancellationToken cancellationToken = default)
    {
        var supplier = ResolveSupplier(supplierName);
        _logger.LogInformation("ConsolidationLayer: Forwarding BookSeats to SupplierLayer for '{Supplier}'", supplier.SupplierName);
        var response = await supplier.BookSeatsAsync(request, cancellationToken);
        _logger.LogInformation("ConsolidationLayer: Received BookSeats response from '{Supplier}'", supplier.SupplierName);
        return response;
    }

    public async Task<BusTicketResponse> GetTicketAsync(BusTicketRequest request, string? supplierName = null, CancellationToken cancellationToken = default)
    {
        var supplier = ResolveSupplier(supplierName);
        _logger.LogInformation("ConsolidationLayer: Forwarding GetTicket to SupplierLayer for '{Supplier}'", supplier.SupplierName);
        var response = await supplier.GetTicketAsync(request, cancellationToken);
        _logger.LogInformation("ConsolidationLayer: Received GetTicket response from '{Supplier}'", supplier.SupplierName);
        return response;
    }

    public async Task<BusTrackingResponse> GetTrackingAsync(BusTrackingRequest request, string? supplierName = null, CancellationToken cancellationToken = default)
    {
        var supplier = ResolveSupplier(supplierName);
        _logger.LogInformation("ConsolidationLayer: Forwarding GetTracking to SupplierLayer for '{Supplier}'", supplier.SupplierName);
        var response = await supplier.GetTrackingAsync(request, cancellationToken);
        _logger.LogInformation("ConsolidationLayer: Received GetTracking response from '{Supplier}'", supplier.SupplierName);
        return response;
    }

    public async Task<BusGetTicketDetailsResponse> GetTicketDetailsAsync(string referanceNumber, string? supplierName = null, CancellationToken cancellationToken = default)
    {
        var supplier = ResolveSupplier(supplierName);
        _logger.LogInformation("ConsolidationLayer: Forwarding GetTicketDetails to SupplierLayer for '{Supplier}'", supplier.SupplierName);
        var response = await supplier.GetTicketDetailsAsync(referanceNumber, cancellationToken);
        _logger.LogInformation("ConsolidationLayer: Received GetTicketDetails response from '{Supplier}'", supplier.SupplierName);
        return response;
    }

    public async Task<BusPreCancelResponse> PreCancelAsync(BusPreCancelRequest request, string? supplierName = null, CancellationToken cancellationToken = default)
    {
        var supplier = ResolveSupplier(supplierName);
        _logger.LogInformation("ConsolidationLayer: Forwarding PreCancel to SupplierLayer for '{Supplier}'", supplier.SupplierName);
        var response = await supplier.PreCancelAsync(request, cancellationToken);
        _logger.LogInformation("ConsolidationLayer: Received PreCancel response from '{Supplier}'", supplier.SupplierName);
        return response;
    }

    public async Task<BusCancelResponse> CancelAsync(BusCancelRequest request, string? supplierName = null, CancellationToken cancellationToken = default)
    {
        var supplier = ResolveSupplier(supplierName);
        _logger.LogInformation("ConsolidationLayer: Forwarding Cancel to SupplierLayer for '{Supplier}'", supplier.SupplierName);
        var response = await supplier.CancelAsync(request, cancellationToken);
        _logger.LogInformation("ConsolidationLayer: Received Cancel response from '{Supplier}'", supplier.SupplierName);
        return response;
    }

    public async Task<BusBalanceResponse> GetBalanceAsync(string? supplierName = null, CancellationToken cancellationToken = default)
    {
        var supplier = ResolveSupplier(supplierName);
        _logger.LogInformation("ConsolidationLayer: Forwarding GetBalance to SupplierLayer for '{Supplier}'", supplier.SupplierName);
        var response = await supplier.GetBalanceAsync(cancellationToken);
        _logger.LogInformation("ConsolidationLayer: Received GetBalance response from '{Supplier}'", supplier.SupplierName);
        return response;
    }

    public async Task<CdnCitiesResponse> GetCdnCitiesAsync(string? supplierName = null, CancellationToken cancellationToken = default)
    {
        var supplier = ResolveSupplier(supplierName);
        _logger.LogInformation("ConsolidationLayer: Forwarding GetCdnCities to SupplierLayer for '{Supplier}'", supplier.SupplierName);
        var response = await supplier.GetCdnCitiesAsync(cancellationToken);
        _logger.LogInformation("ConsolidationLayer: Received GetCdnCities response from '{Supplier}'", supplier.SupplierName);
        return response;
    }
}
