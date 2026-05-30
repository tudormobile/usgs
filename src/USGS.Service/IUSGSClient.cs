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
    /// <param name="httpClient">The <see cref="HttpClient"/> used to make requests. If BaseAddress is not set, uses the default USGS base URL.</param>
    /// <param name="logger">An optional logger. Defaults to a no-op logger if not provided.</param>
    /// <returns>A configured <see cref="IUSGSClient"/> instance.</returns>
    public static IUSGSClient Create(string apiKey, HttpClient httpClient, ILogger? logger = null)
    {
        var options = new USGSClientOptions { ApiKey = apiKey };
        return new USGSClient(options, httpClient, logger);
    }

    /// <summary>
    /// Creates a new <see cref="IUSGSClient"/> instance with custom options.
    /// </summary>
    /// <param name="options">The <see cref="USGSClientOptions"/> to configure the client.</param>
    /// <param name="httpClient">The <see cref="HttpClient"/> used to make requests.</param>
    /// <param name="logger">An optional logger. Defaults to a no-op logger if not provided.</param>
    /// <returns>A configured <see cref="IUSGSClient"/> instance.</returns>
    public static IUSGSClient Create(USGSClientOptions options, HttpClient httpClient, ILogger? logger = null)
        => new USGSClient(options, httpClient, logger);

    /// <summary>
    /// Retrieves daily items from the USGS API for a specific monitoring location and parameter.
    /// </summary>
    /// <param name="monitoringLocationId">The USGS monitoring location identifier (e.g., "01646500").</param>
    /// <param name="parameterCode">The <see cref="USGSParameterCode"/> identifying the measured phenomenon.</param>
    /// <param name="startDate">Optional start date for the data range. If not specified, retrieves all available data.</param>
    /// <param name="endDate">Optional end date for the data range. If not specified, retrieves up to the present.</param>
    /// <param name="limit">Optional maximum number of items to return. If not specified, uses the configured <see cref="USGSClientOptions.MaxItems"/> value.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="USGSCollectionResponse"/> containing the retrieved data or error information.</returns>
    public Task<USGSCollectionResponse> GetDailyItemsAsync(
        string monitoringLocationId,
        USGSParameterCode parameterCode,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int? limit = null,
        CancellationToken cancellationToken = default);
}
