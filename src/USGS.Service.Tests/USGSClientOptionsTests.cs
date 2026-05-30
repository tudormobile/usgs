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
    }
}
