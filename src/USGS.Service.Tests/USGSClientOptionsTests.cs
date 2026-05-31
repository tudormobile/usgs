namespace USGS.Service.Tests;

[TestClass]
public class USGSClientOptionsTests
{
    [TestMethod]
    public void DefaultValues_ShouldBeCorrect()
    {
        var options = new USGSClientOptions();

        Assert.AreEqual(string.Empty, options.ApiKey);
        Assert.AreEqual("https://api.waterdata.usgs.gov/ogcapi/v0", options.BaseAddress);
        Assert.AreEqual(1000, options.MaxItems);
        Assert.AreEqual(30, options.TimeoutSeconds);
        Assert.IsNotNull(options.AdditionalHeaders);
        Assert.AreEqual(0, options.AdditionalHeaders.Count);
    }

    [TestMethod]
    public void AdditionalHeaders_ShouldAllowMultipleHeaders()
    {
        var options = new USGSClientOptions
        {
            ApiKey = "test-key",
            AdditionalHeaders = new Dictionary<string, string>
            {
                { "X-Custom-Header", "custom-value" },
                { "X-Another-Header", "another-value" }
            }
        };

        Assert.AreEqual(2, options.AdditionalHeaders.Count);
        Assert.AreEqual("custom-value", options.AdditionalHeaders["X-Custom-Header"]);
        Assert.AreEqual("another-value", options.AdditionalHeaders["X-Another-Header"]);
    }
}
