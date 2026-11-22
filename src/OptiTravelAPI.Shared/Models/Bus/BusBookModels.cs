using System.Text.Json;
using System.Text.Json.Serialization;

namespace OptiTravelAPI.Shared.Models.Bus;

public class BusBookTripItem
{
    [JsonPropertyName("refNo")]
    public string RefNo { get; set; } = string.Empty;

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public class BusBookRequest
{
    [JsonPropertyName("trips")]
    public List<BusBookTripItem> Trips { get; set; } = new();
}

public class BusBookTripResult
{
    [JsonPropertyName("error")]
    public SupplierError? Error { get; set; }

    [JsonPropertyName("api")]
    public string? Api { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("refNo")]
    public string? RefNo { get; set; }

    [JsonPropertyName("tktNo")]
    public string? TktNo { get; set; }

    [JsonPropertyName("pnrNo")]
    public string? PnrNo { get; set; }

    [JsonPropertyName("tktKey")]
    public string? TktKey { get; set; }

    [JsonPropertyName("operatorNumber")]
    public string? OperatorNumber { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public class BusBookResponse
{
    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }

    [JsonPropertyName("error")]
    public SupplierError? Error { get; set; }

    [JsonPropertyName("trips")]
    public List<BusBookTripResult>? Trips { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}
