namespace OptiTravelAPI.Shared.Interfaces;

/// <summary>
/// Contract for objects that carry or track a distributed trace identifier.
/// </summary>
public interface ITraceable
{
    string TraceId { get; set; }
}
