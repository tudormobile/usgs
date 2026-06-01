namespace USGS.Service.Tests;

[TestClass]
public class ApiResponseTests
{
    [TestMethod]
    public void ApiResponse_Constructor_SetsProperties()
    {
        // Arrange
        var data = new List<string> { "item1", "item2" };
        // Act
        var response = ApiResponse.Success<List<string>>(data);
        // Assert
        Assert.IsTrue(response.IsSuccess);
        Assert.AreEqual(data, response.Data);
    }

    [TestMethod]
    public void ApiResponse_Failure_SetsIsSuccessToFalse()
    {
        // Arrange
        var errorMessage = "An error occurred";
        // Act
        var response = ApiResponse.Failure<string>(errorMessage);
        // Assert
        Assert.IsFalse(response.IsSuccess);
        Assert.AreEqual(errorMessage, response.Data);
    }
}
