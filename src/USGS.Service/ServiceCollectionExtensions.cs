using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

namespace Tudormobile.USGS.Service;

/// <summary>Extension methods for registering USGS services with dependency injection.</summary>
public static class ServiceCollectionExtensions
{
    private readonly static string API_KEY_NAME = "x-api-key";

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
            var logger = sp.GetService<ILogger<USGSClient>>();
            return new USGSClient(options.ApiKey, httpClient, logger);
        });

        return services;
    }
}
