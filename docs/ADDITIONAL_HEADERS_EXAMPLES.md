# Additional Headers Examples

This document provides practical examples of using the `AdditionalHeaders` feature in the USGS Client.

## Dependency Injection Configuration

### Example 1: Simple Tracking Headers

```json
{
  "USGSClient": {
	"ApiKey": "your-api-key-here",
	"AdditionalHeaders": {
	  "X-Application-Name": "WeatherDashboard",
	  "X-Application-Version": "1.2.3"
	}
  }
}
```

### Example 2: Distributed Tracing

```json
{
  "USGSClient": {
	"ApiKey": "your-api-key-here",
	"AdditionalHeaders": {
	  "X-Correlation-Id": "abc-123-def",
	  "X-Request-Source": "BackgroundJob",
	  "User-Agent": "MyWeatherService/2.0"
	}
  }
}
```

### Example 3: Environment-Specific Headers

**appsettings.Development.json:**
```json
{
  "USGSClient": {
	"ApiKey": "dev-api-key",
	"AdditionalHeaders": {
	  "X-Environment": "Development",
	  "X-Debug-Mode": "true"
	}
  }
}
```

**appsettings.Production.json:**
```json
{
  "USGSClient": {
	"ApiKey": "prod-api-key",
	"AdditionalHeaders": {
	  "X-Environment": "Production",
	  "X-Region": "us-west-2"
	}
  }
}
```

## Direct Instantiation Examples

### Example 1: Console Application with Tracking

```csharp
using Tudormobile.USGS.Service;

var options = new USGSClientOptions
{
	ApiKey = "your-api-key",
	AdditionalHeaders = new Dictionary<string, string>
	{
		{ "X-Application", "USGSDataCollector" },
		{ "X-Version", "1.0.0" },
		{ "X-User-Id", Environment.UserName }
	}
};

using var httpClient = new HttpClient
{
	BaseAddress = new Uri(options.BaseAddress),
	Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds)
};

httpClient.DefaultRequestHeaders.Add("x-api-key", options.ApiKey);
foreach (var header in options.AdditionalHeaders)
{
	httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
}

var client = IUSGSClient.Create(options, httpClient);

// Use the client...
var result = await client.GetDailyItemsAsync("12345678", USGSParameterCode.Temperature);
```

### Example 2: Dynamic Correlation ID

```csharp
using Tudormobile.USGS.Service;

// Generate a unique correlation ID for each request
var correlationId = Guid.NewGuid().ToString();

var options = new USGSClientOptions
{
	ApiKey = "your-api-key",
	AdditionalHeaders = new Dictionary<string, string>
	{
		{ "X-Correlation-Id", correlationId },
		{ "X-Request-Time", DateTime.UtcNow.ToString("O") }
	}
};

using var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Add("x-api-key", options.ApiKey);
foreach (var header in options.AdditionalHeaders)
{
	httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
}

var client = IUSGSClient.Create(options, httpClient);
Console.WriteLine($"Making request with correlation ID: {correlationId}");

var result = await client.GetDailyItemsAsync("12345678", USGSParameterCode.Temperature);
```

### Example 3: Proxy Authentication

```csharp
using Tudormobile.USGS.Service;

var options = new USGSClientOptions
{
	ApiKey = "your-api-key",
	AdditionalHeaders = new Dictionary<string, string>
	{
		{ "X-Proxy-Authorization", "Bearer proxy-token-123" },
		{ "X-Organization-Id", "org-456" }
	}
};

using var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Add("x-api-key", options.ApiKey);
foreach (var header in options.AdditionalHeaders)
{
	httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
}

var client = IUSGSClient.Create(options, httpClient);
var result = await client.GetDailyItemsAsync("12345678", USGSParameterCode.Temperature);
```

## Best Practices

1. **Don't duplicate the API key**: When using `AddUSGSClient` (DI), the `x-api-key` header is added from the `ApiKey` option. Don’t add it again in `AdditionalHeaders` (and for direct instantiation, add `x-api-key` to the provided `HttpClient`).

2. **Use meaningful header names**: Prefix custom headers with `X-` to indicate they're extension headers.

3. **Keep sensitive data secure**: Don't log or display header values that contain credentials or tokens.

4. **Document your headers**: If your application requires specific headers, document them for maintainability.

5. **Consider header size**: HTTP headers have size limits (typically 8KB total). Don't add excessively large header values.

## Common Use Cases

| Use Case | Example Headers |
|----------|----------------|
| **Distributed Tracing** | `X-Correlation-Id`, `X-Request-Id`, `X-Trace-Id` |
| **Application Identity** | `User-Agent`, `X-Application-Name`, `X-Application-Version` |
| **Environment Info** | `X-Environment`, `X-Region`, `X-Datacenter` |
| **Debugging** | `X-Debug-Mode`, `X-Verbose-Logging` |
| **Rate Limiting** | `X-Client-Id`, `X-Rate-Limit-Group` |
| **Multi-tenancy** | `X-Tenant-Id`, `X-Organization-Id` |
| **Custom Auth** | `X-Proxy-Authorization`, `X-Secondary-Token` |
