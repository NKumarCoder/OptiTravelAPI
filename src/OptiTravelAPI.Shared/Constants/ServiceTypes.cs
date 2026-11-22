namespace OptiTravelAPI.Shared.Constants;

/// <summary>
/// Supported travel service types across the platform.
/// </summary>
public static class ServiceTypes
{
    public const string Bus = "Bus";
    public const string Hotel = "Hotel";
    public const string Flight = "Flight";
    public const string Cab = "Cab";
    public const string Transfers = "Transfers";

    public static readonly IReadOnlyList<string> All = new[]
    {
        Bus,
        Hotel,
        Flight,
        Cab,
        Transfers
    };
}
