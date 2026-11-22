namespace OptiTravelAPI.Shared.Exceptions;

/// <summary>
/// Base exception for OptiTravelAPI domain errors.
/// </summary>
public class OptiTravelException : Exception
{
    public string? ErrorCode { get; }

    public OptiTravelException(string message, string? errorCode = null) : base(message)
    {
        ErrorCode = errorCode;
    }

    public OptiTravelException(string message, Exception innerException, string? errorCode = null)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}
