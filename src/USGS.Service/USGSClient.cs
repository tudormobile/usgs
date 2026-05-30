using Microsoft.Extensions.Logging;
using System.Web;
using Tudormobile.GeoJSON;

namespace Tudormobile.USGS.Service;

/// <inheritdoc/>
internal class USGSClient : IUSGSClient
{
    private readonly string _apiKey;
    private readonly ILogger _logger;
    private readonly HttpClient _httpClient;

    /// <inheritdoc/>
    public USGSClient(string apiKey, HttpClient httpClient, ILogger? logger = null)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new ArgumentException("API key cannot be null or empty.", nameof(apiKey));
        }
        _apiKey = apiKey;
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
            query["limit"] = limit?.ToString() ?? "1000";

            using var response = await _httpClient.GetStreamAsync($"{result.UrlUsed}?{query}", cancellationToken);

            var doc = await GeoJSONDocument.ParseAsync(response);

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
        catch (Exception ex)
        {
            result.ErrorMessage = ex.Message;
            result.IsSuccess = false;
            result.ErrorKind = USGSErrorKind.QueryConstructionError;
            _logger.LogError(ex, "Error constructing query parameters for USGS API.");
        }

        return result;
    }
}
