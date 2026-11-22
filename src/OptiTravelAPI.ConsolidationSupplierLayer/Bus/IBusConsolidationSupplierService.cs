using OptiTravelAPI.Shared.Models.Bus;

namespace OptiTravelAPI.ConsolidationSupplierLayer.Bus;

/// <summary>
/// Gateway service connecting the API Layer to the Bus Consolidation Layer.
/// </summary>
public interface IBusConsolidationSupplierService
{
    Task<BusLoginResponse> LoginAsync(BusLoginRequest request, string? supplierName = null, CancellationToken cancellationToken = default);
    Task<BusSearchResponse> SearchAsync(BusSearchRequest request, string? supplierName = null, CancellationToken cancellationToken = default);
    Task<BusSeatLayoutResponse> SelectSeatsAsync(BusSeatLayoutRequest request, string? supplierName = null, CancellationToken cancellationToken = default);
    Task<BusBlockResponse> BlockSeatsAsync(BusBlockRequest request, string? supplierName = null, CancellationToken cancellationToken = default);
    Task<BusBookResponse> BookSeatsAsync(BusBookRequest request, string? supplierName = null, CancellationToken cancellationToken = default);
    Task<BusTicketResponse> GetTicketAsync(BusTicketRequest request, string? supplierName = null, CancellationToken cancellationToken = default);
    Task<BusTrackingResponse> GetTrackingAsync(BusTrackingRequest request, string? supplierName = null, CancellationToken cancellationToken = default);
    Task<BusGetTicketDetailsResponse> GetTicketDetailsAsync(string referanceNumber, string? supplierName = null, CancellationToken cancellationToken = default);
    Task<BusPreCancelResponse> PreCancelAsync(BusPreCancelRequest request, string? supplierName = null, CancellationToken cancellationToken = default);
    Task<BusCancelResponse> CancelAsync(BusCancelRequest request, string? supplierName = null, CancellationToken cancellationToken = default);
    Task<BusBalanceResponse> GetBalanceAsync(string? supplierName = null, CancellationToken cancellationToken = default);
    Task<CdnCitiesResponse> GetCdnCitiesAsync(string? supplierName = null, CancellationToken cancellationToken = default);
}
