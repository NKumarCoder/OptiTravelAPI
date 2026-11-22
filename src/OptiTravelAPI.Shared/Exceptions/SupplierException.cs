namespace OptiTravelAPI.Shared.Exceptions;

/// <summary>
/// Exception thrown during communication with an upstream supplier.
/// </summary>
public class SupplierException : OptiTravelException
{
    public string SupplierName { get; }
    public string ServiceType { get; }
    public int? UpstreamStatusCode { get; }
    public string? RawResponseBody { get; }

    public SupplierException(
        string supplierName,
        string serviceType,
        string message,
        int? upstreamStatusCode = null,
        string? errorCode = null,
        string? rawResponseBody = null)
        : base($"Supplier '{supplierName}' for service '{serviceType}' failed: {message}", errorCode)
    {
        SupplierName = supplierName;
        ServiceType = serviceType;
        UpstreamStatusCode = upstreamStatusCode;
        RawResponseBody = rawResponseBody;
    }

    public SupplierException(
        string supplierName,
        string serviceType,
        string message,
        Exception innerException,
        int? upstreamStatusCode = null,
        string? errorCode = null,
        string? rawResponseBody = null)
        : base($"Supplier '{supplierName}' for service '{serviceType}' failed: {message}", innerException, errorCode)
    {
        SupplierName = supplierName;
        ServiceType = serviceType;
        UpstreamStatusCode = upstreamStatusCode;
        RawResponseBody = rawResponseBody;
    }
}
