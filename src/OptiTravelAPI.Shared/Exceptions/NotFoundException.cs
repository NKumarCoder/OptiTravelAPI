namespace OptiTravelAPI.Shared.Exceptions;

/// <summary>
/// Exception thrown when a requested resource or supplier is not found.
/// </summary>
public class NotFoundException : OptiTravelException
{
    public NotFoundException(string message, string? errorCode = "NOT_FOUND")
        : base(message, errorCode)
    {
    }
}
