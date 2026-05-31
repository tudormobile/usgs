namespace Tudormobile.USGS.Service;

/// <summary>
/// Represents a single data point from the USGS API.
/// </summary>
public record USGSItem
{
    /// <summary>
    /// Gets the timestamp of the measurement.
    /// </summary>
    public DateTime Time { get; init; }

    /// <summary>
    /// Gets the measured value.
    /// </summary>
    public double Value { get; init; }
}
