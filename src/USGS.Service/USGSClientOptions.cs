namespace Tudormobile.USGS.Service;

/// <summary>
/// Options for configuring the USGS client.
/// </summary>
public class USGSClientOptions
{
    internal const string USGS_BASE_URL = "https://api.waterdata.usgs.gov/ogcapi/v0";

    /// <summary>
    /// The configuration section name to bind from.
    /// </summary>
    public const string SectionName = "USGSService:USGSClient";

    /// <summary>
    /// The USGS API key.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// The base address for the USGS API.
    /// </summary>
    public string BaseAddress { get; set; } = USGS_BASE_URL;

    /// <summary>
    /// The maximum number of items to return in a single API response.
    /// </summary>
    public int MaxItems { get; set; } = 1000;

    /// <summary>
    /// The timeout for API requests, in seconds.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Optional additional headers to include in all API requests.
    /// The key is the header name, and the value is the header value.
    /// </summary>
    public Dictionary<string, string> AdditionalHeaders { get; set; } = new();

}
