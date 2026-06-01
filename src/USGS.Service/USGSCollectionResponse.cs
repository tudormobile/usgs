namespace Tudormobile.USGS.Service;

/// <summary>
/// Represents the response from a USGS API collection query.
/// </summary>
public class USGSCollectionResponse
{
    /// <summary>
    /// Gets or sets the kind of error that occurred, if any.
    /// </summary>
    public USGSErrorKind ErrorKind { get; set; }

    /// <summary>
    /// Gets or sets the error message, if an error occurred.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or initializes the monitoring location identifier (e.g., "USGS-01646500").
    /// </summary>
    public string MonitoringLocation { get; init; } = "USGS-";

    /// <summary>
    /// Gets or initializes the URL that was used to make the API request.
    /// </summary>
    public string UrlUsed { get; init; } = string.Empty;

    /// <summary>
    /// Gets or initializes the parameter code that was queried.
    /// </summary>
    public USGSParameterCode ParameterCode { get; init; } = USGSParameterCode.Missing;

    /// <summary>
    /// Gets or sets the timestamp of the response or data collection.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the units of measurement for the returned values.
    /// </summary>
    public string Units { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the collection of data items returned by the API.
    /// </summary>
    public List<USGSItem> Items { get; set; } = [];
}
