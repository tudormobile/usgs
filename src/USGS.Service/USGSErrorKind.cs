namespace Tudormobile.USGS.Service;

/// <summary>
/// Specifies the kind of error that occurred during a USGS API request.
/// </summary>
public enum USGSErrorKind
{
    /// <summary>
    /// No error occurred.
    /// </summary>
    None,

    /// <summary>
    /// A network-related error occurred (e.g., connection timeout, DNS failure).
    /// </summary>
    Network,

    /// <summary>
    /// The request was unauthorized (e.g., invalid or missing API key).
    /// </summary>
    Unauthorized,

    /// <summary>
    /// The requested resource was not found (e.g., invalid monitoring location ID).
    /// </summary>
    NotFound,

    /// <summary>
    /// An error occurred while parsing the API response.
    /// </summary>
    ParseError,

    /// <summary>
    /// An unknown or unexpected error occurred.
    /// </summary>
    Unknown,

    /// <summary>
    /// An error occurred while constructing the query parameters.
    /// </summary>
    QueryConstructionError
}