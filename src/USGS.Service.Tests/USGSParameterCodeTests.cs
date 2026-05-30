using Tudormobile.USGS.Service;

namespace USGS.Service.Tests;

[TestClass]
public class USGSParameterCodeTests
{
    [TestMethod]
    public void WellKnownCodes_ShouldHaveCorrectCodeStrings()
    {
        Assert.AreEqual("00060", USGSParameterCode.Discharge.Code);
        Assert.AreEqual("00065", USGSParameterCode.GageHeight.Code);
        Assert.AreEqual("00010", USGSParameterCode.WaterTemperature.Code);
        Assert.AreEqual("00300", USGSParameterCode.DissolvedOxygen.Code);
        Assert.AreEqual("00400", USGSParameterCode.pH.Code);
        Assert.AreEqual("00095", USGSParameterCode.SpecificConductance.Code);
        Assert.AreEqual("63680", USGSParameterCode.Turbidity.Code);
    }

    [TestMethod]
    public void WellKnownCodes_ShouldHaveDescriptions()
    {
        Assert.IsFalse(string.IsNullOrWhiteSpace(USGSParameterCode.Discharge.Description));
        Assert.IsFalse(string.IsNullOrWhiteSpace(USGSParameterCode.GageHeight.Description));
        Assert.IsFalse(string.IsNullOrWhiteSpace(USGSParameterCode.WaterTemperature.Description));
        Assert.IsFalse(string.IsNullOrWhiteSpace(USGSParameterCode.DissolvedOxygen.Description));
        Assert.IsFalse(string.IsNullOrWhiteSpace(USGSParameterCode.pH.Description));
        Assert.IsFalse(string.IsNullOrWhiteSpace(USGSParameterCode.SpecificConductance.Description));
        Assert.IsFalse(string.IsNullOrWhiteSpace(USGSParameterCode.Turbidity.Description));
    }

    [TestMethod]
    public void Custom_ShouldCreateCodeWithGivenValues()
    {
        var code = USGSParameterCode.Custom("12345", "My custom parameter");
        Assert.AreEqual("12345", code.Code);
        Assert.AreEqual("My custom parameter", code.Description);
    }

    [TestMethod]
    public void Custom_ShouldAllowEmptyDescription()
    {
        var code = USGSParameterCode.Custom("12345");
        Assert.AreEqual("12345", code.Code);
        Assert.AreEqual(string.Empty, code.Description);
    }

    [TestMethod]
    public void ToString_ShouldReturnCode()
    {
        Assert.AreEqual("00060", USGSParameterCode.Discharge.ToString());
        Assert.AreEqual("99999", USGSParameterCode.Custom("99999").ToString());
    }
}
