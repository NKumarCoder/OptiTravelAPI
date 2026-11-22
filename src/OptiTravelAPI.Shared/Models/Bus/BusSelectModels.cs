using System.Text.Json;
using System.Text.Json.Serialization;

namespace OptiTravelAPI.Shared.Models.Bus;

public class BusSeatLayoutExt
{
    [JsonPropertyName("timeout")]
    public int? Timeout { get; set; }

    [JsonPropertyName("layout")]
    public string? Layout { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public class BusSeatLayoutRequest
{
    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }

    [JsonPropertyName("busId")]
    public string BusId { get; set; } = string.Empty;

    [JsonPropertyName("bpid")]
    public string? Bpid { get; set; }

    [JsonPropertyName("dpid")]
    public string? Dpid { get; set; }

    [JsonPropertyName("ext")]
    public BusSeatLayoutExt? Ext { get; set; }
}

public class BusSeatItem
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("key")]
    public string? Key { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("desc")]
    public string? Desc { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("x")]
    public int? X { get; set; }

    [JsonPropertyName("y")]
    public int? Y { get; set; }

    [JsonPropertyName("z")]
    public int? Z { get; set; }

    [JsonPropertyName("width")]
    public int? Width { get; set; }

    [JsonPropertyName("height")]
    public int? Height { get; set; }

    [JsonPropertyName("fare")]
    public BusFare? Fare { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public class BusSelectRequired
{
    [JsonPropertyName("idProof")]
    public bool? IdProof { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public class BusSeatLayoutResponse
{
    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }

    [JsonPropertyName("error")]
    public SupplierError? Error { get; set; }

    [JsonPropertyName("api")]
    public string? Api { get; set; }

    [JsonPropertyName("tripKey")]
    public string? TripKey { get; set; }

    [JsonPropertyName("maxSeatsPerTicket")]
    public int? MaxSeatsPerTicket { get; set; }

    [JsonPropertyName("layout")]
    public string? Layout { get; set; }

    [JsonPropertyName("seats")]
    public List<BusSeatItem>? Seats { get; set; }

    [JsonPropertyName("boarding")]
    public List<BusPoint>? Boarding { get; set; }

    [JsonPropertyName("dropping")]
    public List<BusPoint>? Dropping { get; set; }

    [JsonPropertyName("required")]
    public BusSelectRequired? Required { get; set; }

    [JsonPropertyName("verified")]
    public bool? Verified { get; set; }

    [JsonPropertyName("summary")]
    public BusTripSummary? Summary { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}
