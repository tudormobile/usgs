using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Web;
using Tudormobile.GeoJSON;

namespace Tudormobile.USGS.Service;

/// <inheritdoc/>
internal class USGSClient : IUSGSClient
{
    private readonly USGSClientOptions _options;
    private readonly ILogger _logger;
    private readonly HttpClient _httpClient;

    /// <inheritdoc/>
    public USGSClient(USGSClientOptions options, HttpClient httpClient, ILogger? logger = null)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            throw new ArgumentException("API key cannot be null or empty.", nameof(options));
        }
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger.LogDebug("USGSClient initialized.");
    }

    /// <inheritdoc/>
    public async Task<USGSCollectionResponse> GetDailyItemsAsync(string monitoringLocationId, USGSParameterCode parameterCode, DateTime? startDate = null, DateTime? endDate = null, int? limit = null, CancellationToken cancellationToken = default)
    {
        var baseUrl = _httpClient.BaseAddress?.ToString() ?? USGSClientOptions.USGS_BASE_URL;
        var result = new USGSCollectionResponse()
        {
            MonitoringLocation = $"USGS-{monitoringLocationId}",
            ParameterCode = parameterCode,
            Timestamp = DateTime.UtcNow,
            UrlUsed = $"{baseUrl}/collections/daily/items"
        };

        try
        {
            var start = startDate?.ToString("yyyy-MM-dd") ?? "..";
            var end = endDate?.ToString("yyyy-MM-dd") ?? "..";
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["parameter_code"] = result.ParameterCode.Code;
            query["monitoring_location_id"] = result.MonitoringLocation;
            query["f"] = "json";
            query["datetime"] = $"{start}/{end}";
            query["limit"] = limit?.ToString() ?? _options.MaxItems.ToString();

            using var response = await _httpClient.GetAsync($"{result.UrlUsed}?{query}", HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            // Check HTTP status code
            if (!response.IsSuccessStatusCode)
            {
                result.IsSuccess = false;
                result.ErrorMessage = $"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}";

                result.ErrorKind = response.StatusCode switch
                {
                    System.Net.HttpStatusCode.Unauthorized => USGSErrorKind.Unauthorized,
                    System.Net.HttpStatusCode.NotFound => USGSErrorKind.NotFound,
                    _ => USGSErrorKind.Network
                };

                _logger.LogError("USGS API request failed with status code {StatusCode}: {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
                return result;
            }

            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var doc = await GeoJSONDocument.ParseAsync(stream);

            result.IsSuccess = true;
            if (DateTime.TryParse(doc.Objects["timeStamp"]?.ToString(), out var timestamp))
            {
                result.Timestamp = timestamp;
            }
            var units = doc.FeatureCollection.Features.FirstOrDefault()?.Properties["unit_of_measure"];
            if (units != null && units.HasValue)
            {
                result.Units = units.Value.ToString();
            }
            foreach (var feature in doc.FeatureCollection.Features)
            {
                double.TryParse(feature.Properties["value"].GetString(), out var v);
                feature.Properties["time"].TryGetDateTime(out var t);
                result.Items.Add(new USGSItem
                {
                    Value = v,
                    Time = t
                });
            }
        }
        catch (HttpRequestException ex)
        {
            result.ErrorMessage = ex.Message;
            result.IsSuccess = false;
            result.ErrorKind = USGSErrorKind.Network;
            _logger.LogError(ex, "Network error occurred while calling USGS API.");
        }
        catch (JsonException ex)
        {
            result.ErrorMessage = ex.Message;
            result.IsSuccess = false;
            result.ErrorKind = USGSErrorKind.ParseError;
            _logger.LogError(ex, "Error parsing JSON response from USGS API.");
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.Message;
            result.IsSuccess = false;
            result.ErrorKind = USGSErrorKind.Unknown;
            _logger.LogError(ex, "Unexpected error occurred while calling USGS API.");
        }

        return result;
    }
}
