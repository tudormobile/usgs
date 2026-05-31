# USGS Client Usage Patterns

This document describes how to use the USGS Client in both dependency injection and direct instantiation scenarios.

## Pattern 1: Dependency Injection (Recommended for ASP.NET Core / .NET applications)

### Configuration (appsettings.json)
```json
{
  "USGSClient": {
	"ApiKey": "your-api-key-here",
	"BaseAddress": "https://api.waterdata.usgs.gov/ogcapi/v0",
	"MaxItems": 1000,
	"TimeoutSeconds": 30,
	"AdditionalHeaders": {
	  "X-Custom-Header": "custom-value",
	  "X-Request-Id": "unique-request-id",
	  "User-Agent": "MyApp/1.0"
	}
  }
}
```

**Note:** The `AdditionalHeaders` section is optional. You can include zero or more custom headers.

### Registration (Program.cs or Startup.cs)
```csharp
using Tudormobile.USGS.Service;

var builder = WebApplication.CreateBuilder(args);

// Register USGS Client with DI container
builder.Services.AddUSGSClient(builder.Configuration);
```

### Usage in Services/Controllers
```csharp
public class WeatherService
{
	private readonly IUSGSClient _usgsClient;

	public WeatherService(IUSGSClient usgsClient)
	{
		_usgsClient = usgsClient;
	}

	public async Task GetDataAsync()
	{
		var result = await _usgsClient.GetDailyItemsAsync(
			monitoringLocationId: "12345678",
			parameterCode: USGSParameterCode.Temperature,
			startDate: DateTime.Now.AddDays(-30),
			endDate: DateTime.Now
		);
	}
}
```

## Pattern 2: Direct Instantiation (For console apps, libraries, or simple scenarios)

### Option A: Simple Creation (minimal configuration)
```csharp
using Tudormobile.USGS.Service;

// Create HttpClient (consider using IHttpClientFactory in real apps)
using var httpClient = new HttpClient
{
	BaseAddress = new Uri("https://api.waterdata.usgs.gov/ogcapi/v0")
};
httpClient.DefaultRequestHeaders.Add("x-api-key", "your-api-key");

// Create client with simple API key
var client = IUSGSClient.Create("your-api-key", httpClient);

var result = await client.GetDailyItemsAsync("12345678", USGSParameterCode.Temperature);
```

### Option B: Full Configuration (with all options)
```csharp
using Tudormobile.USGS.Service;

// Configure options
var options = new USGSClientOptions
{
	ApiKey = "your-api-key",
	BaseAddress = "https://api.waterdata.usgs.gov/ogcapi/v0",
	MaxItems = 2000,  // Custom max items
	TimeoutSeconds = 60,  // Custom timeout
	AdditionalHeaders = new Dictionary<string, string>
	{
		{ "X-Custom-Header", "custom-value" },
		{ "User-Agent", "MyConsoleApp/1.0" }
	}
};

// Create HttpClient
using var httpClient = new HttpClient
{
	BaseAddress = new Uri(options.BaseAddress),
	Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds)
};
httpClient.DefaultRequestHeaders.Add("x-api-key", options.ApiKey);

// Add any additional headers
foreach (var header in options.AdditionalHeaders)
{
	httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
}

// Create client with full options
var client = IUSGSClient.Create(options, httpClient);

var result = await client.GetDailyItemsAsync("12345678", USGSParameterCode.Temperature);
```

## Key Benefits

Both patterns now share the same `USGSClientOptions` configuration model:

- **DI Pattern**: Best for applications with dependency injection containers
  - Configuration from appsettings.json
  - Automatic HttpClient lifecycle management
  - Singleton instance reuse
  - Full logging integration
  - Additional headers configured via JSON

- **Direct Instantiation**: Best for simple scenarios or libraries
  - No DI container required
  - Full control over HttpClient lifecycle
  - Choose between simple (API key only) or full configuration
  - Same options model as DI pattern
  - Additional headers set programmatically

Both patterns respect the same configuration options: `ApiKey`, `BaseAddress`, `MaxItems`, `TimeoutSeconds`, and `AdditionalHeaders`.

## Additional Headers Use Cases

The `AdditionalHeaders` feature is useful for:
- **Tracking**: Add correlation IDs or request IDs for distributed tracing
- **Authentication**: Include additional auth tokens or credentials
- **Custom behavior**: Send custom headers required by proxies or middleware
- **User-Agent**: Specify your application name and version for API analytics
- **Debugging**: Add debug flags or environment identifiers

Example scenarios:
```json
{
  "USGSClient": {
    "ApiKey": "your-key",
    "AdditionalHeaders": {
      "X-Correlation-Id": "{{guid}}",
      "X-Environment": "Production",
      "User-Agent": "WeatherApp/2.1.0"
    }
  }
}
```
