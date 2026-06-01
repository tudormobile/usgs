namespace USGS.Service.Tests;

[TestCategory("Integration")]
[TestClass]
public class IntegationTests
{
    [TestMethod]
    [Ignore("Integration test - requires live USGS API")]
    public async Task USGSClient_GetDailyItemsAsync_ReturnsData()
    {
        // Arrange
        var monitoringLocationId = "425048073472501"; // Example monitoring location ID
        var parameterCode = USGSParameterCode.WaterLevel; // Example parameter code for water level
        var startDate = DateTime.Now.AddDays(-30);
        var endDate = DateTime.Now;
        using var httpClient = new HttpClient();
        var apiKey = Environment.GetEnvironmentVariable("USGS_API_KEY") ?? "your_api_key_here";
        var client = IUSGSClient.Create(apiKey, httpClient);

        // Act
        var result = await client.GetDailyItemsAsync(monitoringLocationId, parameterCode, startDate, endDate, cancellationToken: TestContext.CancellationToken);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(USGSErrorKind.None, result.ErrorKind);
        Assert.IsNotEmpty(result.Items);
        Assert.AreEqual("ft", result.Units);
    }

    public TestContext TestContext { get; set; }    // Set by MSTest framework
}