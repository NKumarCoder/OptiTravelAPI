using OptiTravelAPI.Shared.Models.Bus;
using OptiTravelAPI.SupplierLayer.Abstractions;

namespace OptiTravelAPI.SupplierLayer.Bus;

/// <summary>
/// Abstraction for a Bus service supplier (e.g. TBO, RedBus, Abhibus).
/// </summary>
public interface IBusSupplier : ISupplier
{
    Task<BusLoginResponse> LoginAsync(BusLoginRequest request, CancellationToken cancellationToken = default);
    Task<BusSearchResponse> SearchAsync(BusSearchRequest request, CancellationToken cancellationToken = default);
    Task<BusSeatLayoutResponse> SelectSeatsAsync(BusSeatLayoutRequest request, CancellationToken cancellationToken = default);
    Task<BusBlockResponse> BlockSeatsAsync(BusBlockRequest request, CancellationToken cancellationToken = default);
    Task<BusBookResponse> BookSeatsAsync(BusBookRequest request, CancellationToken cancellationToken = default);
    Task<BusTicketResponse> GetTicketAsync(BusTicketRequest request, CancellationToken cancellationToken = default);
    Task<BusTrackingResponse> GetTrackingAsync(BusTrackingRequest request, CancellationToken cancellationToken = default);
    Task<BusGetTicketDetailsResponse> GetTicketDetailsAsync(string referanceNumber, CancellationToken cancellationToken = default);
    Task<BusPreCancelResponse> PreCancelAsync(BusPreCancelRequest request, CancellationToken cancellationToken = default);
    Task<BusCancelResponse> CancelAsync(BusCancelRequest request, CancellationToken cancellationToken = default);
    Task<BusBalanceResponse> GetBalanceAsync(CancellationToken cancellationToken = default);
    Task<CdnCitiesResponse> GetCdnCitiesAsync(CancellationToken cancellationToken = default);
}
