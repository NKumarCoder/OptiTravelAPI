using System.Text.Json;
using System.Text.Json.Serialization;

namespace OptiTravelAPI.Shared.Models.Bus;

/// <summary>
/// Upstream supplier error details.
/// </summary>
public class SupplierError
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("desc")]
    public string? Desc { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

/// <summary>
/// Boarding or dropping point description.
/// </summary>
public class BusPoint
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("time")]
    public DateTime? Time { get; set; }

    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    [JsonPropertyName("address")]
    public string? Address { get; set; }

    [JsonPropertyName("landmark")]
    public string? Landmark { get; set; }

    [JsonPropertyName("location")]
    public string? Location { get; set; }

    [JsonPropertyName("lat")]
    public double? Lat { get; set; }

    [JsonPropertyName("lon")]
    public double? Lon { get; set; }

    [JsonPropertyName("mainPointID")]
    public string? MainPointID { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

/// <summary>
/// Breakdown of fares, taxes, charges and commissions.
/// </summary>
public class BusFare
{
    [JsonPropertyName("total")]
    public decimal? Total { get; set; }

    [JsonPropertyName("base")]
    public decimal? Base { get; set; }

    [JsonPropertyName("gst")]
    public decimal? Gst { get; set; }

    [JsonPropertyName("toll")]
    public decimal? Toll { get; set; }

    [JsonPropertyName("sCharge")]
    public decimal? SCharge { get; set; }

    [JsonPropertyName("levyCharge")]
    public decimal? LevyCharge { get; set; }

    [JsonPropertyName("txnFee")]
    public decimal? TxnFee { get; set; }

    [JsonPropertyName("tMarkUp")]
    public decimal? TMarkUp { get; set; }

    [JsonPropertyName("commission")]
    public decimal? Commission { get; set; }

    [JsonPropertyName("totalNetFare")]
    public decimal? TotalNetFare { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public class PolicyRule
{
    [JsonPropertyName("span")]
    public string? Span { get; set; }

    [JsonPropertyName("unit")]
    public string? Unit { get; set; }

    [JsonPropertyName("rate")]
    public string? Rate { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public class BusPolicy
{
    [JsonPropertyName("cancel")]
    public List<PolicyRule>? Cancel { get; set; }

    [JsonPropertyName("reschedule")]
    public List<PolicyRule>? Reschedule { get; set; }

    [JsonPropertyName("other")]
    public Dictionary<string, string>? Other { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public class BusTags
{
    [JsonPropertyName("ac")]
    public bool? Ac { get; set; }

    [JsonPropertyName("nonAc")]
    public bool? NonAc { get; set; }

    [JsonPropertyName("seater")]
    public bool? Seater { get; set; }

    [JsonPropertyName("sleeper")]
    public bool? Sleeper { get; set; }

    [JsonPropertyName("liveTracking")]
    public bool? LiveTracking { get; set; }

    [JsonPropertyName("mTicket")]
    public bool? MTicket { get; set; }

    [JsonPropertyName("cancel")]
    public bool? Cancel { get; set; }

    [JsonPropertyName("partialCancel")]
    public bool? PartialCancel { get; set; }

    [JsonPropertyName("rtc")]
    public bool? Rtc { get; set; }

    [JsonPropertyName("refund")]
    public bool? Refund { get; set; }

    [JsonPropertyName("reschedule")]
    public bool? Reschedule { get; set; }

    [JsonPropertyName("safety")]
    public bool? Safety { get; set; }

    [JsonPropertyName("singleLadies")]
    public bool? SingleLadies { get; set; }

    [JsonPropertyName("socialDistance")]
    public bool? SocialDistance { get; set; }

    [JsonPropertyName("washroom")]
    public bool? Washroom { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public class BusRequired
{
    [JsonPropertyName("poi")]
    public bool? Poi { get; set; }

    [JsonPropertyName("bdp")]
    public bool? Bdp { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public class BusOptional
{
    [JsonPropertyName("gst")]
    public bool? Gst { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public class BusTripSummary
{
    [JsonPropertyName("src")]
    public string? Src { get; set; }

    [JsonPropertyName("dst")]
    public string? Dst { get; set; }

    [JsonPropertyName("doj")]
    public DateTime? Doj { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}
