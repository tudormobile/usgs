using Microsoft.Extensions.Logging;

namespace Tudormobile.USGS.Service;

/// <summary>
/// Represents a client for interacting with the USGS API.
/// </summary>
public interface IUSGSClient
{
    /// <summary>
    /// Creates a new <see cref="IUSGSClient"/> instance.
    /// </summary>
    /// <param name="apiKey">The USGS API key. Must not be null or whitespace.</param>
    /// <param name="httpClient">The <see cref="HttpClient"/> used to make requests.</param>
    /// <param name="logger">An optional logger. Defaults to a no-op logger if not provided.</param>
    /// <returns>A configured <see cref="IUSGSClient"/> instance.</returns>
    public static IUSGSClient Create(string apiKey, HttpClient httpClient, ILogger? logger = null)
        => new USGSClient(apiKey, httpClient, logger);

    public Task<USGSCollectionResponse> GetDailyItemsAsync(
        string monitoringLocationId,
        USGSParameterCode parameterCode,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int? limit = null,
        CancellationToken cancellationToken = default);
}

public class USGSCollectionResponse
{
    public bool IsSuccess { get; set; }
    public USGSErrorKind ErrorKind { get; set; }
    public string? ErrorMessage { get; set; }
    public string MonitoringLocation { get; init; }
    public string UrlUsed { get; init; }
    public USGSParameterCode ParameterCode { get; init; }
    public DateTime Timestamp { get; set; }
    public string Units { get; set; } = string.Empty;
    public List<USGSItem> Items { get; set; } = [];
}

public record USGSItem
{
    public DateTime Time { get; init; }
    public double Value { get; init; }
}

public enum USGSErrorKind
{
    None,
    Network,
    Unauthorized,
    NotFound,
    ParseError,
    Unknown,
    QueryConstructionError
}