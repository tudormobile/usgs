using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tudormobile.USGS.Service;

namespace USGS.Service.Tests;

[TestClass]
public class ServiceCollectionExtensionsTests
{
    private static IConfiguration BuildConfig(string apiKey = "test-key", string? baseAddress = null)
    {
        var values = new Dictionary<string, string?>
        {
            [$"{USGSClientOptions.SectionName}:{nameof(USGSClientOptions.ApiKey)}"] = apiKey,
        };
        if (baseAddress is not null)
            values[$"{USGSClientOptions.SectionName}:{nameof(USGSClientOptions.BaseAddress)}"] = baseAddress;

        return new ConfigurationBuilder().AddInMemoryCollection(values).Build();
    }

    [TestMethod]
    public void AddUSGSClient_ShouldRegisterIUSGSClientAsSingleton()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddUSGSClient(BuildConfig());

        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IUSGSClient));
        Assert.IsNotNull(descriptor);
        Assert.AreEqual(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [TestMethod]
    public void AddUSGSClient_ShouldResolveIUSGSClient()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddUSGSClient(BuildConfig());

        using var provider = services.BuildServiceProvider();
        var client = provider.GetService<IUSGSClient>();
        Assert.IsNotNull(client);
    }

    [TestMethod]
    public void AddUSGSClient_ShouldReturnSameInstanceEachTime()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddUSGSClient(BuildConfig());

        using var provider = services.BuildServiceProvider();
        var client1 = provider.GetRequiredService<IUSGSClient>();
        var client2 = provider.GetRequiredService<IUSGSClient>();
        Assert.AreSame(client1, client2);
    }
}
