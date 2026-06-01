using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Tudormobile.USGS.Service;

/// <summary>
/// Represents the USGS API service, which can be used to proxy incoming requests to USGS endpoints.
/// </summary>
public class USGSApi
{
    private readonly string _apiKey;
    private readonly IUSGSClient _usgsClient;
    private readonly ILogger _logger;
    private readonly IWebHostEnvironment _env;

    private static readonly Lazy<string> _version = new(() =>
    {
        var v = Assembly.GetExecutingAssembly().GetName().Version;
        return v == null ? "0.0.0" : $"{v.Major}.{v.Minor}.{v.Build}";
    });

    /// <summary>
    /// Initializes a new instance of the <see cref="USGSApi"/> class.
    /// </summary>
    /// <param name="apiKey">The API key used for authentication.</param>
    /// <param name="usgsClient">The USGS client used to make API requests.</param>
    /// <param name="logger">The logger used for logging.</param>
    /// <param name="env">The web host environment.</param>
    public USGSApi(string apiKey, IUSGSClient usgsClient, ILogger logger, IWebHostEnvironment env)
    {
        _apiKey = apiKey;
        _usgsClient = usgsClient;
        _logger = logger;
        _env = env;
    }

    internal Task<IResult> GetVersionAsync(HttpContext context, string apiKey)
        => HandleApiRequest(context, apiKey, nameof(GetVersionAsync), async () =>
        {
            return Results.Ok(ApiResponse.Success($"USGS Service {_version.Value} (API v1.0)"));
        });

    internal Task<IResult> GetDailyValues(HttpContext context, string apiKey, string location, string parameter, DateTime? startDate, DateTime? endDate)
        => HandleApiRequest(context, apiKey, nameof(GetDailyValues), async () =>
        {
            var dailyItems = await _usgsClient.GetDailyItemsAsync(location, USGSParameterCode.Custom(parameter), startDate, endDate);
            return dailyItems.ErrorKind == USGSErrorKind.None ? Results.Ok(ApiResponse.Success(dailyItems)) : Results.Ok(ApiResponse.Failure(dailyItems));
        });

    private async Task<IResult> HandleApiRequest(HttpContext context, string apiKey, string callerName, Func<Task<IResult>> onAuthorized)
    {
        LogApiRequest(context, callerName);
        if (_env.IsDevelopment() || (!string.IsNullOrWhiteSpace(_apiKey) && apiKey == _apiKey))
            try
            {
                return await onAuthorized();
            }
            catch (Exception ex)
            {
                LogException(context, ex, callerName);
                return Results.Ok(ApiResponse.Failure(ex.Message));
            }
        }
        var redactedKey = apiKey.Length > 4 ? $"{apiKey[..4]}..." : "???";
        _logger.LogError("USGSService, {CallerName}, {RemoteIpAddress}, {ApiKey}, INVALID API KEY", callerName, context.Connection.RemoteIpAddress, redactedKey);
        return Results.Unauthorized();
    }

    private void LogApiRequest(HttpContext context, [CallerMemberName] string callerName = "")
    {
        _logger.LogInformation("USGSService, {CallerName}, {RemoteIpAddress}",
            callerName, context.Connection.RemoteIpAddress);
    }

    private void LogException(HttpContext context, Exception ex, [CallerMemberName] string callerName = "")
    {
        _logger.LogError(ex, "USGSService, {CallerName}, {RemoteIpAddress}",
            callerName, context.Connection.RemoteIpAddress);
    }


}
