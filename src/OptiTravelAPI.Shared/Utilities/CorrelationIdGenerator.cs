namespace OptiTravelAPI.Shared.Utilities;

/// <summary>
/// Utilities for generating and managing distributed correlation/trace IDs.
/// </summary>
public static class CorrelationIdGenerator
{
    /// <summary>
    /// Generates a new unique correlation ID.
    /// </summary>
    public static string Generate() => Guid.NewGuid().ToString("N");
}
