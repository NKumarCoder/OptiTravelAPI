using System.Text.Json;
using System.Text.Json.Serialization;

namespace OptiTravelAPI.Shared.Models.Bus;

public class BusSearchTripRequest
{
    [JsonPropertyName("src")]
    public string Src { get; set; } = string.Empty;

    [JsonPropertyName("dst")]
    public string Dst { get; set; } = string.Empty;

    [JsonPropertyName("doj")]
    public string Doj { get; set; } = string.Empty;
}

public class BusSearchExt
{
    [JsonPropertyName("timeout")]
    public int? Timeout { get; set; }

    [JsonPropertyName("filters")]
    public bool? Filters { get; set; }

    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public class BusSearchRequest
{
    [JsonPropertyName("trips")]
    public List<BusSearchTripRequest> Trips { get; set; } = new();

    [JsonPropertyName("ext")]
    public BusSearchExt? Ext { get; set; }
}

public class BusSeatAvailability
{
    [JsonPropertyName("total")]
    public int? Total { get; set; }

    [JsonPropertyName("avlAll")]
    public int? AvlAll { get; set; }

    [JsonPropertyName("avlWindow")]
    public int? AvlWindow { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public class BusItem
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("nmbr")]
    public string? Nmbr { get; set; }

    [JsonPropertyName("timeD")]
    public DateTime? TimeD { get; set; }

    [JsonPropertyName("timeA")]
    public DateTime? TimeA { get; set; }

    [JsonPropertyName("busStartTime")]
    public string? BusStartTime { get; set; }

    [JsonPropertyName("duration")]
    public int? Duration { get; set; }

    [JsonPropertyName("rating")]
    public double? Rating { get; set; }

    [JsonPropertyName("nextDay")]
    public bool? NextDay { get; set; }

    [JsonPropertyName("seats")]
    public BusSeatAvailability? Seats { get; set; }

    [JsonPropertyName("fares")]
    public List<BusFare>? Fares { get; set; }

    [JsonPropertyName("policy")]
    public BusPolicy? Policy { get; set; }

    [JsonPropertyName("tags")]
    public BusTags? Tags { get; set; }

    [JsonPropertyName("amenities")]
    public List<string>? Amenities { get; set; }

    [JsonPropertyName("reviews")]
    public List<string>? Reviews { get; set; }

    [JsonPropertyName("photos")]
    public List<string>? Photos { get; set; }

    [JsonPropertyName("videos")]
    public List<string>? Videos { get; set; }

    [JsonPropertyName("required")]
    public BusRequired? Required { get; set; }

    [JsonPropertyName("optional")]
    public BusOptional? Optional { get; set; }

    [JsonPropertyName("boarding")]
    public List<BusPoint>? Boarding { get; set; }

    [JsonPropertyName("dropping")]
    public List<BusPoint>? Dropping { get; set; }

    [JsonPropertyName("api")]
    public string? Api { get; set; }

    [JsonPropertyName("apiId")]
    public int? ApiId { get; set; }

    [JsonPropertyName("ecode")]
    public string? Ecode { get; set; }

    [JsonPropertyName("key")]
    public string? Key { get; set; }

    [JsonPropertyName("verified")]
    public bool? Verified { get; set; }

    [JsonPropertyName("partialCancellation")]
    public bool? PartialCancellation { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public class BusSearchFilters
{
    [JsonPropertyName("buses")]
    public int? Buses { get; set; }

    [JsonPropertyName("seaters")]
    public int? Seaters { get; set; }

    [JsonPropertyName("sleeper")]
    public int? Sleeper { get; set; }

    [JsonPropertyName("ac")]
    public int? Ac { get; set; }

    [JsonPropertyName("nonAc")]
    public int? NonAc { get; set; }

    [JsonPropertyName("liveTracking")]
    public int? LiveTracking { get; set; }

    [JsonPropertyName("mTicket")]
    public int? MTicket { get; set; }

    [JsonPropertyName("cancel")]
    public int? Cancel { get; set; }

    [JsonPropertyName("partialCancel")]
    public int? PartialCancel { get; set; }

    [JsonPropertyName("rtc")]
    public int? Rtc { get; set; }

    [JsonPropertyName("refund")]
    public int? Refund { get; set; }

    [JsonPropertyName("reschedule")]
    public int? Reschedule { get; set; }

    [JsonPropertyName("safety")]
    public int? Safety { get; set; }

    [JsonPropertyName("singleLadies")]
    public int? SingleLadies { get; set; }

    [JsonPropertyName("socialDistance")]
    public int? SocialDistance { get; set; }

    [JsonPropertyName("washroom")]
    public int? Washroom { get; set; }

    [JsonPropertyName("timeDRange")]
    public List<DateTime>? TimeDRange { get; set; }

    [JsonPropertyName("timeARange")]
    public List<DateTime>? TimeARange { get; set; }

    [JsonPropertyName("priceRange")]
    public List<decimal>? PriceRange { get; set; }

    [JsonPropertyName("ratingRange")]
    public List<double>? RatingRange { get; set; }

    [JsonPropertyName("timings")]
    public Dictionary<string, int>? Timings { get; set; }

    [JsonPropertyName("travels")]
    public Dictionary<string, int>? Travels { get; set; }

    [JsonPropertyName("boarding")]
    public Dictionary<string, int>? Boarding { get; set; }

    [JsonPropertyName("dropping")]
    public Dictionary<string, int>? Dropping { get; set; }

    [JsonPropertyName("amenities")]
    public Dictionary<string, int>? Amenities { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public class BusSearchTripResult
{
    [JsonPropertyName("error")]
    public SupplierError? Error { get; set; }

    [JsonPropertyName("buses")]
    public List<BusItem>? Buses { get; set; }

    [JsonPropertyName("filters")]
    public BusSearchFilters? Filters { get; set; }

    [JsonPropertyName("summary")]
    public BusTripSummary? Summary { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public class BusSearchResponse
{
    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }

    [JsonPropertyName("error")]
    public SupplierError? Error { get; set; }

    [JsonPropertyName("trips")]
    public List<BusSearchTripResult>? Trips { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}
