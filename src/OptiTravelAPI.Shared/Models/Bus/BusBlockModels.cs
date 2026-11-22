using System.Text.Json;
using System.Text.Json.Serialization;

namespace OptiTravelAPI.Shared.Models.Bus;

public class BusBlockTripItem
{
    [JsonPropertyName("tripKey")]
    public string TripKey { get; set; } = string.Empty;

    [JsonPropertyName("bpid")]
    public string Bpid { get; set; } = string.Empty;

    [JsonPropertyName("dpid")]
    public string Dpid { get; set; } = string.Empty;

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public class BusPoc
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("phone")]
    public string Phone { get; set; } = string.Empty;

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public class BusPassengerBlock
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("gender")]
    public string Gender { get; set; } = string.Empty;

    [JsonPropertyName("age")]
    public int Age { get; set; }

    [JsonPropertyName("seatIds")]
    public List<string> SeatIds { get; set; } = new();

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public class BusBlockExt
{
    [JsonPropertyName("timeout")]
    public int? Timeout { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public class BusBlockRequest
{
    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }

    [JsonPropertyName("trips")]
    public List<BusBlockTripItem> Trips { get; set; } = new();

    [JsonPropertyName("poc")]
    public BusPoc Poc { get; set; } = new();

    [JsonPropertyName("pax")]
    public List<BusPassengerBlock> Pax { get; set; } = new();

    [JsonPropertyName("ext")]
    public BusBlockExt? Ext { get; set; }
}

public class BusFareUpdate
{
    [JsonPropertyName("prev")]
    public BusFare? Prev { get; set; }

    [JsonPropertyName("updt")]
    public BusFare? Updt { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public class BusBlockTripResult
{
    [JsonPropertyName("api")]
    public string? Api { get; set; }

    [JsonPropertyName("error")]
    public SupplierError? Error { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("blockingId")]
    public string? BlockingId { get; set; }

    [JsonPropertyName("refNo")]
    public string? RefNo { get; set; }

    [JsonPropertyName("blockKey")]
    public string? BlockKey { get; set; }

    [JsonPropertyName("span")]
    public int? Span { get; set; }

    [JsonPropertyName("payType")]
    public string? PayType { get; set; }

    [JsonPropertyName("fareUpdate")]
    public BusFareUpdate? FareUpdate { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public class BusBlockResponse
{
    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }

    [JsonPropertyName("error")]
    public SupplierError? Error { get; set; }

    [JsonPropertyName("trips")]
    public List<BusBlockTripResult>? Trips { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}
