using Microsoft.Extensions.Logging.Testing;

namespace USGS.Service.Tests;

[TestClass]
public class USGSClientTests
{
    [TestMethod]
    public void Create_ShouldReturnUSGSClientInstance()
    {
        // Arrange
        var apiKey = "test-api-key";
        using var httpClient = new HttpClient();
        var logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<USGSClient>();

        // Act
        var client = new USGSClient(apiKey, httpClient, logger);

        // Assert
        Assert.IsNotNull(client);
    }

    [TestMethod]
    public void CreateThroughInterface_ShouldReturnUSGSClientInstance()
    {
        // Arrange
        var apiKey = "test-api-key";
        using var httpClient = new HttpClient();
        var logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<USGSClient>();

        // Act
        IUSGSClient client = IUSGSClient.Create(apiKey, httpClient, logger);

        // Assert
        Assert.IsNotNull(client);
    }

    [TestMethod]
    public void Constructor_NullOrEmptyApiKey_ShouldThrow()
    {
        using var httpClient = new HttpClient();
        var ex = Assert.ThrowsExactly<ArgumentException>(() => new USGSClient("", httpClient));
        Assert.AreEqual("apiKey", ex.ParamName);
        ex = Assert.ThrowsExactly<ArgumentException>(() => new USGSClient("   ", httpClient));
        Assert.AreEqual("apiKey", ex.ParamName);
    }

    [TestMethod]
    public void Constructor_NullHttpClient_ShouldThrow()
    {
        var ex = Assert.ThrowsExactly<ArgumentNullException>(() => new USGSClient("key", null!));
        Assert.AreEqual("httpClient", ex.ParamName);
    }

    [TestMethod]
    public void Constructor_ShouldLogDebugMessageOnCreation()
    {
        var logger = new FakeLogger<USGSClient>();
        using var httpClient = new HttpClient();
        var client = new USGSClient("test-api-key", httpClient, logger);

        Assert.AreEqual(1, logger.Collector.Count);
        Assert.AreEqual(LogLevel.Debug, logger.Collector.LatestRecord.Level);
        Assert.AreEqual("USGSClient initialized.", logger.Collector.LatestRecord.Message);
    }

}
