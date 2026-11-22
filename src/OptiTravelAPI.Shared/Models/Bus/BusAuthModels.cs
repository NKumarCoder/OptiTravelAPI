using System.Text.Json;
using System.Text.Json.Serialization;

namespace OptiTravelAPI.Shared.Models.Bus;

/// <summary>
/// Request payload for supplier authentication.
/// </summary>
public class BusLoginRequest
{
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Response payload for supplier authentication.
/// </summary>
public class BusLoginResponse
{
    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }

    [JsonPropertyName("error")]
    public SupplierError? Error { get; set; }

    [JsonPropertyName("accessToken")]
    public string? AccessToken { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}
