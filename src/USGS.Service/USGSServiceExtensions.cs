using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

namespace Tudormobile.USGS.Service;

/// <summary>Extension methods for registering USGS services with dependency injection.</summary>
public static class USGSServiceExtensions
{
    private readonly static string API_KEY_NAME = "x-api-key";

    /// <summary>
    /// Adds the USGS service to the application builder, enabling middleware for handling USGS-related requests.
    /// </summary>
    /// <param name="app">The web application used to configure the HTTP pipeline.</param>
    /// <returns>The web application configured with USGS services.</returns>
    public static WebApplication UseUSGSService(this WebApplication app)
    {
        var prefix = "/usgs/v1";

        // Obtain the configured client to use for handling requests
        var usgsClient = app.Services.GetRequiredService<IUSGSClient>();

        // Configure the service using the Service configuration section
        var section = app.Configuration.GetSection("USGSService");
        var serviceApiKey = section.GetValue<string>("ApiKey") ?? string.Empty;
        var serviceRoot = section.GetValue<string>("ServiceRoot")?.TrimEnd('/') ?? string.Empty;

        var api = new USGSApi(
            serviceApiKey,
            usgsClient,
            app.Logger,
            app.Environment);

        // Map USGS endpoints

        app.MapGet($"{serviceRoot}{prefix}/status", (HttpContext context, [FromHeader(Name = "ApiKey")] string? apiKey)
            => api.GetVersionAsync(context, apiKey ?? string.Empty));

        app.MapGet($"{serviceRoot}{prefix}/{{location}}/{{parameter}}/daily", (HttpContext context, [FromHeader(Name = "ApiKey")] string? apiKey, string location, string parameter, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
            => api.GetDailyValues(context, apiKey ?? string.Empty, location, parameter, startDate, endDate));

        app.Logger.LogInformation("USGSService, Running, {Prefix}", serviceRoot + prefix);
        return app;
    }

    /// <summary>
    /// Adds the USGS client to the service collection, binding configuration from
    /// the <c>USGSClient</c> section of <paramref name="configuration"/>.
    /// </summary>
    public static IServiceCollection AddUSGSClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = configuration
            .GetSection(USGSClientOptions.SectionName)
            .Get<USGSClientOptions>() ?? new USGSClientOptions();

        services.AddHttpClient(nameof(IUSGSClient), client =>
        {
            client.BaseAddress = new Uri(options.BaseAddress);
            client.DefaultRequestHeaders.Add(API_KEY_NAME, options.ApiKey);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);

            // Add any additional headers from configuration
            foreach (var header in options.AdditionalHeaders)
            {
                if (string.IsNullOrWhiteSpace(header.Key) || client.DefaultRequestHeaders.Contains(header.Key))
                    continue;

                client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
            }
        })
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            AllowAutoRedirect = true,          // already the default
            MaxAutomaticRedirections = 10,     // tune as needed
            UseCookies = false                 // not needed for an API client
        });

        services.AddSingleton<IUSGSClient>(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = factory.CreateClient(nameof(IUSGSClient));
            var logger = sp.GetService<ILogger<IUSGSClient>>();
            return new USGSClient(options, httpClient, logger);
        });

        services.AddOutputCache();
        return services;
    }
}
