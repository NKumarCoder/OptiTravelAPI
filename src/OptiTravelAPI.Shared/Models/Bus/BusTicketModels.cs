using System.Text.Json;
using System.Text.Json.Serialization;

namespace OptiTravelAPI.Shared.Models.Bus;

public class BusTicketRequest
{
    [JsonPropertyName("tktNo")]
    public string TktNo { get; set; } = string.Empty;
}

public class BusTicketBusInfo
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("src")]
    public string? Src { get; set; }

    [JsonPropertyName("dst")]
    public string? Dst { get; set; }

    [JsonPropertyName("timeD")]
    public DateTime? TimeD { get; set; }

    [JsonPropertyName("timeA")]
    public DateTime? TimeA { get; set; }

    [JsonPropertyName("duration")]
    public int? Duration { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public class BusTicketContact
{
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    [JsonPropertyName("address")]
    public string? Address { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public class BusTicketPaxItem
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("gender")]
    public string? Gender { get; set; }

    [JsonPropertyName("age")]
    public int? Age { get; set; }

    [JsonPropertyName("seatId")]
    public string? SeatId { get; set; }

    [JsonPropertyName("fare")]
    public BusFare? Fare { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public class BusTicketResponse
{
    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }

    [JsonPropertyName("error")]
    public SupplierError? Error { get; set; }

    [JsonPropertyName("api")]
    public string? Api { get; set; }

    [JsonPropertyName("pnr")]
    public string? Pnr { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("bookedDate")]
    public DateTime? BookedDate { get; set; }

    [JsonPropertyName("bus")]
    public BusTicketBusInfo? Bus { get; set; }

    [JsonPropertyName("stopB")]
    public BusPoint? StopB { get; set; }

    [JsonPropertyName("stopD")]
    public BusPoint? StopD { get; set; }

    [JsonPropertyName("contact")]
    public BusTicketContact? Contact { get; set; }

    [JsonPropertyName("pax")]
    public List<BusTicketPaxItem>? Pax { get; set; }

    [JsonPropertyName("ticketNo")]
    public string? TicketNo { get; set; }

    [JsonPropertyName("liveTracking")]
    public bool? LiveTracking { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public class BusTicketDetailsPoint
{
    [JsonPropertyName("pointId")]
    public string? PointId { get; set; }

    [JsonPropertyName("location")]
    public string? Location { get; set; }

    [JsonPropertyName("address")]
    public string? Address { get; set; }

    [JsonPropertyName("landMark")]
    public string? LandMark { get; set; }

    [JsonPropertyName("time")]
    public string? Time { get; set; }

    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public class BusPassengerDetail
{
    [JsonPropertyName("age")]
    public int? Age { get; set; }

    [JsonPropertyName("firstName")]
    public string? FirstName { get; set; }

    [JsonPropertyName("seatNo")]
    public string? SeatNo { get; set; }

    [JsonPropertyName("gender")]
    public string? Gender { get; set; }

    [JsonPropertyName("ladiesSeat")]
    public bool? LadiesSeat { get; set; }

    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("idType")]
    public string? IdType { get; set; }

    [JsonPropertyName("idNumber")]
    public string? IdNumber { get; set; }

    [JsonPropertyName("nameOnId")]
    public string? NameOnId { get; set; }

    [JsonPropertyName("primary")]
    public bool? Primary { get; set; }

    [JsonPropertyName("ac")]
    public bool? Ac { get; set; }

    [JsonPropertyName("sleeper")]
    public bool? Sleeper { get; set; }

    [JsonPropertyName("isPrimaryPax")]
    public bool? IsPrimaryPax { get; set; }

    [JsonPropertyName("isLadiesSeat")]
    public bool? IsLadiesSeat { get; set; }

    [JsonPropertyName("isSleeper")]
    public bool? IsSleeper { get; set; }

    [JsonPropertyName("baseFare")]
    public decimal? BaseFare { get; set; }

    [JsonPropertyName("gst")]
    public decimal? Gst { get; set; }

    [JsonPropertyName("serviceCharge")]
    public decimal? ServiceCharge { get; set; }

    [JsonPropertyName("tollCharge")]
    public decimal? TollCharge { get; set; }

    [JsonPropertyName("levyCharge")]
    public decimal? LevyCharge { get; set; }

    [JsonPropertyName("txnFee")]
    public decimal? TxnFee { get; set; }

    [JsonPropertyName("apiBaseFare")]
    public decimal? ApiBaseFare { get; set; }

    [JsonPropertyName("netBaseFare")]
    public decimal? NetBaseFare { get; set; }

    [JsonPropertyName("total")]
    public decimal? Total { get; set; }

    [JsonPropertyName("status")]
    public int? Status { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public class BusGetTicketDetailsResponse
{
    [JsonPropertyName("userId")]
    public int? UserId { get; set; }

    [JsonPropertyName("holdId")]
    public string? HoldId { get; set; }

    [JsonPropertyName("pnr")]
    public string? Pnr { get; set; }

    [JsonPropertyName("ticketId")]
    public string? TicketId { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("sourceId")]
    public string? SourceId { get; set; }

    [JsonPropertyName("destinationId")]
    public string? DestinationId { get; set; }

    [JsonPropertyName("sourceName")]
    public string? SourceName { get; set; }

    [JsonPropertyName("destinationName")]
    public string? DestinationName { get; set; }

    [JsonPropertyName("journeyDate")]
    public DateTime? JourneyDate { get; set; }

    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }

    [JsonPropertyName("boardingPoint")]
    public BusTicketDetailsPoint? BoardingPoint { get; set; }

    [JsonPropertyName("droppingPoint")]
    public BusTicketDetailsPoint? DroppingPoint { get; set; }

    [JsonPropertyName("emailId")]
    public string? EmailId { get; set; }

    [JsonPropertyName("mobile")]
    public string? Mobile { get; set; }

    [JsonPropertyName("address")]
    public string? Address { get; set; }

    [JsonPropertyName("passangers")]
    public List<BusPassengerDetail>? Passangers { get; set; }

    [JsonPropertyName("operatorName")]
    public string? OperatorName { get; set; }

    [JsonPropertyName("busType")]
    public string? BusType { get; set; }

    [JsonPropertyName("departureTime")]
    public string? DepartureTime { get; set; }

    [JsonPropertyName("arrivalTime")]
    public string? ArrivalTime { get; set; }

    [JsonPropertyName("partialCancellationAllowed")]
    public bool? PartialCancellationAllowed { get; set; }

    [JsonPropertyName("operatorId")]
    public string? OperatorId { get; set; }

    [JsonPropertyName("serviceId")]
    public string? ServiceId { get; set; }

    [JsonPropertyName("mTicketAllowed")]
    public bool? MTicketAllowed { get; set; }

    [JsonPropertyName("isRtc")]
    public bool? IsRtc { get; set; }

    [JsonPropertyName("cancellationPolicy")]
    public string? CancellationPolicy { get; set; }

    [JsonPropertyName("supplierId")]
    public int? SupplierId { get; set; }

    [JsonPropertyName("supplierName")]
    public string? SupplierName { get; set; }

    [JsonPropertyName("key")]
    public string? Key { get; set; }

    [JsonPropertyName("apiFare")]
    public decimal? ApiFare { get; set; }

    [JsonPropertyName("netFare")]
    public decimal? NetFare { get; set; }

    [JsonPropertyName("publishedFare")]
    public decimal? PublishedFare { get; set; }

    [JsonPropertyName("postMarkup")]
    public decimal? PostMarkup { get; set; }

    [JsonPropertyName("isAc")]
    public bool? IsAc { get; set; }

    [JsonPropertyName("ticketKey")]
    public string? TicketKey { get; set; }

    [JsonPropertyName("createdBy")]
    public int? CreatedBy { get; set; }

    [JsonPropertyName("createdOn")]
    public DateTime? CreatedOn { get; set; }

    [JsonPropertyName("modifiedBy")]
    public int? ModifiedBy { get; set; }

    [JsonPropertyName("modifiedOn")]
    public DateTime? ModifiedOn { get; set; }

    [JsonPropertyName("modifiedByDetails")]
    public string? ModifiedByDetails { get; set; }

    [JsonPropertyName("createdByDetails")]
    public string? CreatedByDetails { get; set; }

    [JsonPropertyName("referenceNumber")]
    public string? ReferenceNumber { get; set; }

    [JsonPropertyName("error")]
    public SupplierError? Error { get; set; }

    [JsonPropertyName("oldTicketId")]
    public string? OldTicketId { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}
