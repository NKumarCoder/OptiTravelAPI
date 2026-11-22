namespace OptiTravelAPI.Shared.Exceptions;

/// <summary>
/// Exception thrown during consolidation and aggregation logic.
/// </summary>
public class ConsolidationException : OptiTravelException
{
    public string ServiceType { get; }

    public ConsolidationException(string serviceType, string message, string? errorCode = null)
        : base($"Consolidation failed for service '{serviceType}': {message}", errorCode)
    {
        ServiceType = serviceType;
    }

    public ConsolidationException(string serviceType, string message, Exception innerException, string? errorCode = null)
        : base($"Consolidation failed for service '{serviceType}': {message}", innerException, errorCode)
    {
        ServiceType = serviceType;
    }
}
