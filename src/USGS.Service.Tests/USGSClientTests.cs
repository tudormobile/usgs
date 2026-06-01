using Microsoft.Extensions.Logging.Testing;
using System.Net;

namespace USGS.Service.Tests;

[TestClass]
public class USGSClientTests
{
    [TestMethod]
    public void Create_ShouldReturnUSGSClientInstance()
    {
        // Arrange
        var options = new USGSClientOptions { ApiKey = "test-api-key" };
        using var httpClient = new HttpClient();
        var logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<USGSClient>();

        // Act
        var client = new USGSClient(options, httpClient, logger);

        // Assert
        Assert.IsNotNull(client);
        Assert.IsInstanceOfType<USGSClient>(client);
        Assert.IsInstanceOfType<IUSGSClient>(client);
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
        Assert.IsInstanceOfType<IUSGSClient>(client);
        Assert.IsInstanceOfType<USGSClient>(client);
    }

    [TestMethod]
    public void Constructor_NullOrEmptyApiKey_ShouldThrow()
    {
        using var httpClient = new HttpClient();

        // Test empty string
        var ex = Assert.ThrowsExactly<ArgumentException>(() => new USGSClient(new USGSClientOptions { ApiKey = "" }, httpClient));
        Assert.AreEqual("options", ex.ParamName);
        Assert.Contains("API key", ex.Message);

        // Test whitespace string
        ex = Assert.ThrowsExactly<ArgumentException>(() => new USGSClient(new USGSClientOptions { ApiKey = "   " }, httpClient));
        Assert.AreEqual("options", ex.ParamName);
        Assert.Contains("API key", ex.Message);
    }

    [TestMethod]
    public void Constructor_NullHttpClient_ShouldThrow()
    {
        var options = new USGSClientOptions { ApiKey = "key" };
        var ex = Assert.ThrowsExactly<ArgumentNullException>(() => new USGSClient(options, null!));
        Assert.AreEqual("httpClient", ex.ParamName);
        Assert.IsNotNull(ex.Message);
    }

    [TestMethod]
    public void Constructor_NullOptions_ShouldThrow()
    {
        using var httpClient = new HttpClient();
        var ex = Assert.ThrowsExactly<ArgumentNullException>(() => new USGSClient(null!, httpClient));
        Assert.AreEqual("options", ex.ParamName);
        Assert.IsNotNull(ex.Message);
    }

    [TestMethod]
    public void Constructor_WithNullLogger_ShouldNotThrow()
    {
        // Arrange
        var options = new USGSClientOptions { ApiKey = "test-api-key" };
        using var httpClient = new HttpClient();

        // Act
        var client = new USGSClient(options, httpClient, null);

        // Assert
        Assert.IsNotNull(client);
        Assert.IsInstanceOfType<USGSClient>(client);
    }

    [TestMethod]
    public void CreateThroughInterface_WithOptions_ShouldReturnUSGSClientInstance()
    {
        // Arrange
        var options = new USGSClientOptions
        {
            ApiKey = "test-api-key",
            BaseAddress = "https://test.example.com",
            MaxItems = 500,
            TimeoutSeconds = 60
        };
        using var httpClient = new HttpClient();
        var logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<USGSClient>();

        // Act
        IUSGSClient client = IUSGSClient.Create(options, httpClient, logger);

        // Assert
        Assert.IsNotNull(client);
        Assert.IsInstanceOfType<IUSGSClient>(client);
        Assert.IsInstanceOfType<USGSClient>(client);
    }

    [TestMethod]
    public void Constructor_ShouldLogDebugMessageOnCreation()
    {
        var logger = new FakeLogger<USGSClient>();
        using var httpClient = new HttpClient();
        var options = new USGSClientOptions { ApiKey = "test-api-key" };
        var client = new USGSClient(options, httpClient, logger);

        Assert.AreEqual(1, logger.Collector.Count);
        Assert.AreEqual(LogLevel.Debug, logger.Collector.LatestRecord.Level);
        Assert.AreEqual("USGSClient initialized.", logger.Collector.LatestRecord.Message);
        Assert.IsNotNull(logger.Collector.LatestRecord);
        Assert.IsNotNull(client);
    }

    [TestMethod]
    public async Task GetDailyItemsAsync_ReturnsItems()
    {
        // Arrange
        var startDate = DateTime.Now.AddDays(-30);
        var endDate = DateTime.Now;
        var monitoringLocationId = "some_location-id";
        var parameterCode = USGSParameterCode.pH;
        var limit = 12345;

        var apiKey = "some-api_key";
        var json = @"{
    ""type"":""FeatureCollection"",
    ""features"":[
        {
            ""type"":""Feature"",
            ""properties"":{
                ""id"":""014a2022-73e1-4e30-98f9-0a37eb973970"",
                ""time_series_id"":""0516aa95a1f940e6a549dc60900d709b"",
                ""monitoring_location_id"":""USGS-425048073472501"",
                ""parameter_code"":""72019"",
                ""statistic_id"":""00003"",
                ""time"":""2026-05-12"",
                ""value"":""2.19"",
                ""unit_of_measure"":""ft"",
                ""approval_status"":""Provisional"",
                ""qualifier"":null,
                ""last_modified"":""2026-05-13T07:05:35.063655+00:00""
            },
            ""id"":""014a2022-73e1-4e30-98f9-0a37eb973970"",
            ""geometry"":{
                ""type"":""Point"",
                ""coordinates"":[
                    -73.7908055555556,
                    42.8463888888889
                ]
            }
        },
        {
            ""type"":""Feature"",
            ""properties"":{
                ""id"":""0448991a-a057-48ee-bd21-7564a122d8a7"",
                ""time_series_id"":""0516aa95a1f940e6a549dc60900d709b"",
                ""monitoring_location_id"":""USGS-425048073472501"",
                ""parameter_code"":""72019"",
                ""statistic_id"":""00003"",
                ""time"":""2026-04-19"",
                ""value"":""1.70"",
                ""unit_of_measure"":""ft"",
                ""approval_status"":""Provisional"",
                ""qualifier"":null,
                ""last_modified"":""2026-04-20T07:04:55.093032+00:00""
            },
            ""id"":""0448991a-a057-48ee-bd21-7564a122d8a7"",
            ""geometry"":{
                ""type"":""Point"",
                ""coordinates"":[
                    -73.7908055555556,
                    42.8463888888889
                ]
            }
        },
        {
            ""type"":""Feature"",
            ""properties"":{
                ""id"":""fa4abc2f-69aa-4657-a87e-5702b07c5142"",
                ""time_series_id"":""0516aa95a1f940e6a549dc60900d709b"",
                ""monitoring_location_id"":""USGS-425048073472501"",
                ""parameter_code"":""72019"",
                ""statistic_id"":""00003"",
                ""time"":""2026-05-11"",
                ""value"":""2.06"",
                ""unit_of_measure"":""ft"",
                ""approval_status"":""Provisional"",
                ""qualifier"":null,
                ""last_modified"":""2026-05-12T07:05:31.415674+00:00""
            },
            ""id"":""fa4abc2f-69aa-4657-a87e-5702b07c5142"",
            ""geometry"":{
                ""type"":""Point"",
                ""coordinates"":[
                    -73.7908055555556,
                    42.8463888888889
                ]
            }
        }
    ],
    ""numberReturned"":59,
    ""links"":[
        {
            ""type"":""application/geo+json"",
            ""rel"":""self"",
            ""title"":""This document as GeoJSON"",
            ""href"":""https://api.waterdata.usgs.gov/ogcapi/v0/collections/daily/items?f=json&parameter_code=72019&monitoring_location_id=USGS-425048073472501&datetime=2026-04-01%2F2026-05-30&limit=1000""
        },
        {
            ""rel"":""alternate"",
            ""type"":""application/ld+json"",
            ""title"":""This document as RDF (JSON-LD)"",
            ""href"":""https://api.waterdata.usgs.gov/ogcapi/v0/collections/daily/items?f=jsonld&parameter_code=72019&monitoring_location_id=USGS-425048073472501&datetime=2026-04-01%2F2026-05-30&limit=1000""
        },
        {
            ""type"":""text/html"",
            ""rel"":""alternate"",
            ""title"":""This document as HTML"",
            ""href"":""https://api.waterdata.usgs.gov/ogcapi/v0/collections/daily/items?f=html&parameter_code=72019&monitoring_location_id=USGS-425048073472501&datetime=2026-04-01%2F2026-05-30&limit=1000""
        },
        {
            ""type"":""text/csv; charset=utf-8"",
            ""rel"":""alternate"",
            ""title"":""This document as CSV"",
            ""href"":""https://api.waterdata.usgs.gov/ogcapi/v0/collections/daily/items?f=csv&parameter_code=72019&monitoring_location_id=USGS-425048073472501&datetime=2026-04-01%2F2026-05-30&limit=1000""
        },
        {
            ""type"":""application/json"",
            ""title"":""Daily values"",
            ""rel"":""collection"",
            ""href"":""https://api.waterdata.usgs.gov/ogcapi/v0/collections/daily""
        }
    ],
    ""timeStamp"":""2026-05-30T20:05:34.341299Z""
}";
        var handler = new MockHttpMessageHandler() { JsonResponse = json };
        using var httpClient = new HttpClient(handler);
        var client = IUSGSClient.Create(apiKey, httpClient);

        // Act
        var response = await client.GetDailyItemsAsync(monitoringLocationId, parameterCode, startDate, endDate, limit, TestContext.CancellationToken);

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(USGSErrorKind.None, response.ErrorKind);
        Assert.IsNull(response.ErrorMessage);
        Assert.HasCount(3, response.Items);
        Assert.IsNotNull(response.Items);
        Assert.AreEqual("ft", response.Units);
        Assert.AreEqual(parameterCode, response.ParameterCode);
        Assert.Contains(monitoringLocationId, response.MonitoringLocation);
        Assert.IsNotNull(response.UrlUsed);
        Assert.Contains("collections/daily/items", response.UrlUsed);

        // Verify timestamp was parsed (timeStamp from JSON: "2026-05-30T20:05:34.341299Z")
        Assert.AreNotEqual(default, response.Timestamp);
        Assert.AreEqual(2026, response.Timestamp.Year);
        Assert.AreEqual(5, response.Timestamp.Month);
        Assert.AreEqual(30, response.Timestamp.Day);

        // Verify first item
        var firstItem = response.Items[0];
        Assert.IsNotNull(firstItem);
        Assert.AreEqual(new DateTime(2026, 5, 12), firstItem.Time);
        Assert.AreEqual(2.19, firstItem.Value, 0.001);

        // Verify second item
        var secondItem = response.Items[1];
        Assert.IsNotNull(secondItem);
        Assert.AreEqual(new DateTime(2026, 4, 19), secondItem.Time);
        Assert.AreEqual(1.70, secondItem.Value, 0.001);

        // Verify third item
        var thirdItem = response.Items[2];
        Assert.IsNotNull(thirdItem);
        Assert.AreEqual(new DateTime(2026, 5, 11), thirdItem.Time);
        Assert.AreEqual(2.06, thirdItem.Value, 0.001);

        // Verify items are in the collection
        Assert.IsTrue(response.Items.All(item => item.Time != default));
        Assert.IsTrue(response.Items.All(item => item.Value > 0));
    }

    [TestMethod]
    public async Task GetDailyItemsAsync_WithInternalServerError_ReturnsNetworkErrorResponse()
    {
        // Arrange
        var startDate = DateTime.Now.AddDays(-30);
        var endDate = DateTime.Now;
        var monitoringLocationId = "some_location-id";
        var parameterCode = USGSParameterCode.pH;
        var limit = 12345;

        var apiKey = "some-api_key";
        var handler = new MockHttpMessageHandler() { AlwaysResponds = new HttpResponseMessage(HttpStatusCode.InternalServerError) };
        using var httpClient = new HttpClient(handler);
        var client = IUSGSClient.Create(apiKey, httpClient);

        // Act
        var response = await client.GetDailyItemsAsync(monitoringLocationId, parameterCode, startDate, endDate, limit, TestContext.CancellationToken);

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(USGSErrorKind.Network, response.ErrorKind);
        Assert.IsEmpty(response.Items);
        Assert.IsNotNull(response.ErrorMessage);
    }

    [TestMethod]
    public async Task GetDailyItemsAsync_WithInvalidJson_ReturnsNetworkErrorResponse()
    {
        // Arrange
        var startDate = DateTime.Now.AddDays(-30);
        var endDate = DateTime.Now;
        var monitoringLocationId = "some_location-id";
        var parameterCode = USGSParameterCode.pH;
        var limit = 12345;

        var apiKey = "some-api_key";
        var handler = new MockHttpMessageHandler() { JsonResponse = "this is invalid json" };
        using var httpClient = new HttpClient(handler);
        var client = IUSGSClient.Create(apiKey, httpClient);

        // Act
        var response = await client.GetDailyItemsAsync(monitoringLocationId, parameterCode, startDate, endDate, limit, TestContext.CancellationToken);

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(USGSErrorKind.ParseError, response.ErrorKind);
        Assert.IsEmpty(response.Items);
        Assert.IsNotNull(response.ErrorMessage);
    }

    [TestMethod]
    public async Task GetDailyItemsAsync_WithException_ReturnsNetworkErrorResponse()
    {
        // Arrange
        var startDate = DateTime.Now.AddDays(-30);
        var endDate = DateTime.Now;
        var monitoringLocationId = "some_location-id";
        var parameterCode = USGSParameterCode.pH;
        var limit = 12345;

        var apiKey = "some-api_key";
        var handler = new MockHttpMessageHandler() { AlwaysThrows = new Exception("Some Random Exception") };
        using var httpClient = new HttpClient(handler);
        var client = IUSGSClient.Create(apiKey, httpClient);

        // Act
        var response = await client.GetDailyItemsAsync(monitoringLocationId, parameterCode, startDate, endDate, limit, TestContext.CancellationToken);

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(USGSErrorKind.Unknown, response.ErrorKind);
        Assert.IsEmpty(response.Items);
        Assert.AreEqual("Some Random Exception", response.ErrorMessage);
    }

    [TestMethod]
    public async Task GetDailyItemsAsync_WithNotFound_ReturnsNetworkErrorResponse()
    {
        // Arrange
        var startDate = DateTime.Now.AddDays(-30);
        var endDate = DateTime.Now;
        var monitoringLocationId = "some_location-id";
        var parameterCode = USGSParameterCode.pH;
        var limit = 12345;

        var apiKey = "some-api_key";
        var handler = new MockHttpMessageHandler() { AlwaysResponds = new HttpResponseMessage(HttpStatusCode.NotFound) };
        using var httpClient = new HttpClient(handler);
        var client = IUSGSClient.Create(apiKey, httpClient);

        // Act
        var response = await client.GetDailyItemsAsync(monitoringLocationId, parameterCode, startDate, endDate, limit, TestContext.CancellationToken);

        // Assert
        Assert.IsNotNull(response);
        Assert.IsEmpty(response.Items);
        Assert.AreEqual(USGSErrorKind.NotFound, response.ErrorKind);
    }

    [TestMethod]
    public async Task GetDailyItemsAsync_WithUnauthorized_ReturnsNetworkErrorResponse()
    {
        // Arrange
        var startDate = DateTime.Now.AddDays(-30);
        var endDate = DateTime.Now;
        var monitoringLocationId = "some_location-id";
        var parameterCode = USGSParameterCode.pH;
        var limit = 12345;

        var apiKey = "some-api_key";
        var handler = new MockHttpMessageHandler() { AlwaysResponds = new HttpResponseMessage(HttpStatusCode.Unauthorized) };
        using var httpClient = new HttpClient(handler);
        var client = IUSGSClient.Create(apiKey, httpClient);

        // Act
        var response = await client.GetDailyItemsAsync(monitoringLocationId, parameterCode, startDate, endDate, limit, TestContext.CancellationToken);

        // Assert
        Assert.IsNotNull(response);
        Assert.IsEmpty(response.Items);
        Assert.AreEqual(USGSErrorKind.Unauthorized, response.ErrorKind);
    }

    [TestMethod]
    public async Task GetDailyItemsAsync_WithNetworkFailure_ReturnsNetworkErrorResponse()
    {
        // Arrange
        var startDate = DateTime.Now.AddDays(-30);
        var endDate = DateTime.Now;
        var monitoringLocationId = "some_location-id";
        var parameterCode = USGSParameterCode.pH;
        var limit = 12345;

        var apiKey = "some-api_key";
        var handler = new MockHttpMessageHandler()
        {
            AlwaysThrows = new HttpRequestException("Network unreachable")
        };
        using var httpClient = new HttpClient(handler);
        var client = IUSGSClient.Create(apiKey, httpClient);

        // Act
        var response = await client.GetDailyItemsAsync(monitoringLocationId, parameterCode, startDate, endDate, limit, TestContext.CancellationToken);

        // Assert
        Assert.IsNotNull(response);
        Assert.IsEmpty(response.Items);
        Assert.AreEqual(USGSErrorKind.Network, response.ErrorKind);
        Assert.IsNotNull(response.ErrorMessage);
        Assert.Contains("Network unreachable", response.ErrorMessage);
    }

    public TestContext TestContext { get; set; }    // Set by MSTest 
}
