using System.Text.Json;
using System.Text.Json.Serialization;

namespace OptiTravelAPI.Shared.Models.Bus;

public class BusPreCancelRequest
{
    [JsonPropertyName("tktNo")]
    public string TktNo { get; set; } = string.Empty;

    [JsonPropertyName("seats")]
    public List<string> Seats { get; set; } = new();
}

public class BusPreCancelTicketItem
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("seats")]
    public List<string>? Seats { get; set; }

    [JsonPropertyName("totalFare")]
    public decimal? TotalFare { get; set; }

    [JsonPropertyName("baseFare")]
    public decimal? BaseFare { get; set; }

    [JsonPropertyName("charge")]
    public decimal? Charge { get; set; }

    [JsonPropertyName("refund")]
    public decimal? Refund { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public class BusPreCancelResponse
{
    [JsonPropertyName("error")]
    public SupplierError? Error { get; set; }

    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }

    [JsonPropertyName("api")]
    public string? Api { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    [JsonPropertyName("tickets")]
    public List<BusPreCancelTicketItem>? Tickets { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public class BusCancelRequest
{
    [JsonPropertyName("tktNo")]
    public string TktNo { get; set; } = string.Empty;

    [JsonPropertyName("seats")]
    public List<string> Seats { get; set; } = new();
}

public class BusCancelFareInfo
{
    [JsonPropertyName("totalFare")]
    public decimal? TotalFare { get; set; }

    [JsonPropertyName("baseFare")]
    public decimal? BaseFare { get; set; }

    [JsonPropertyName("charge")]
    public decimal? Charge { get; set; }

    [JsonPropertyName("refund")]
    public decimal? Refund { get; set; }

    [JsonPropertyName("serviceTaxOnCC")]
    public decimal? ServiceTaxOnCC { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public class BusCancelResponse
{
    [JsonPropertyName("error")]
    public SupplierError? Error { get; set; }

    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }

    [JsonPropertyName("api")]
    public string? Api { get; set; }

    [JsonPropertyName("newTicketId")]
    public string? NewTicketId { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("seats")]
    public List<string>? Seats { get; set; }

    [JsonPropertyName("fares")]
    public BusCancelFareInfo? Fares { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}
