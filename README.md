# USGS Client and Proxy Service

Early-stage .NET project for working with the USGS Water Data API.

This repository currently provides:

- A reusable `IUSGSClient` implementation for direct API access.
- A lightweight ASP.NET Core proxy service with simplified endpoints.
- Sample hosts:
	- Console sample for direct API consumption.
	- Web API sample for hosting the proxy service.

## Status

This project is in early development and is not production-ready.

There is no NuGet package at this time. If this project proves generally useful, packaging and a broader proxy API build-out will be added later.

## Repository Layout

- `src/USGS.Service` - Core client and proxy service implementation.
- `src/USGS.Service.Tests` - Unit and integration tests.
- `samples/SimpleConsoleApp` - Console sample calling USGS directly.
- `samples/WebApiHost` - ASP.NET Core sample hosting the proxy service.
- `docs` - Working notes and usage references.

## Requirements

- .NET 10 SDK (the projects target `net10.0`).
- A USGS API key for real API calls.

## Core Concepts

### Direct client access

The client is exposed through `IUSGSClient` and can be created either:

- Through DI with `AddUSGSClient(configuration)`.
- Directly with `IUSGSClient.Create(...)`.

Primary operation:

- `GetDailyItemsAsync(locationId, parameterCode, startDate, endDate, limit, cancellationToken)`

### Proxy service access

The proxy is enabled by calling `UseUSGSService()` in an ASP.NET Core app.

It exposes a simplified API surface and forwards requests through `IUSGSClient`.

## Configuration

The service and client bind from the `USGSService` section.

Example (`appsettings.json`):

```json
{
	"USGSService": {
		"ServiceRoot": "/weather",
		"ApiKey": "replace-with-service-api-key",
		"USGSClient": {
			"ApiKey": "replace-with-client-api-key",
			"BaseAddress": "https://api.waterdata.usgs.gov/ogcapi/v0",
			"MaxItems": 1000,
			"TimeoutSeconds": 30,
			"AdditionalHeaders": {
				"User-Agent": "USGS-Sample/1.0"
			}
		}
	}
}
```

Notes:

- `USGSService:ApiKey` secures incoming proxy requests.
- `USGSService:USGSClient:ApiKey` is used for outbound calls to USGS.
- In Development environment, proxy key checks are bypassed.

## Running the Samples

From repository root:

### 1) Console sample (direct USGS call)

The console sample reads `USGS_API_KEY` from environment variables.

PowerShell:

```powershell
$env:USGS_API_KEY="your-usgs-api-key"
dotnet run --project .\samples\SimpleConsoleApp\SimpleConsoleApp.csproj
```

### 2) Web API sample (proxy host)

Update `samples/WebApiHost/appsettings.json` with real keys, then run:

```powershell
dotnet run --project .\samples\WebApiHost\WebApiHost.csproj
```

Default local URL in launch settings is `http://localhost:5087`.

## Proxy Endpoints

With `ServiceRoot=/weather`, the endpoint prefix becomes:

- `/weather/usgs/v1`

Available endpoints:

- `GET /weather/usgs/v1/status`
	- Returns service version metadata.
- `GET /weather/usgs/v1/{location}/{parameter}/daily?startDate=yyyy-MM-dd&endDate=yyyy-MM-dd`
	- Returns daily items for a monitoring location and parameter code.

Proxy auth header:

- Header name expected by the proxy: `ApiKey`

Example requests:

```powershell
# Status
curl -H "ApiKey: replace-with-service-api-key" `
	http://localhost:5087/weather/usgs/v1/status

# Daily values (location and parameter are path segments)
curl -H "ApiKey: replace-with-service-api-key" `
	"http://localhost:5087/weather/usgs/v1/425048073472501/72019/daily?startDate=2026-05-01&endDate=2026-05-30"
```

## Using the Client in Your App

Minimal DI registration:

```csharp
using Tudormobile.USGS.Service;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddUSGSClient(builder.Configuration);
```

Minimal usage:

```csharp
public class MyService
{
		private readonly IUSGSClient _usgsClient;

		public MyService(IUSGSClient usgsClient)
		{
				_usgsClient = usgsClient;
		}

		public Task<USGSCollectionResponse> GetWaterLevelAsync(CancellationToken ct)
				=> _usgsClient.GetDailyItemsAsync(
						monitoringLocationId: "425048073472501",
						parameterCode: USGSParameterCode.WaterLevel,
						startDate: DateTime.UtcNow.AddDays(-30),
						endDate: DateTime.UtcNow,
						cancellationToken: ct);
}
```

## Testing

Run all tests:

```powershell
dotnet test .\src\USGS.slnx
```

Notes:

- Integration tests are currently marked ignored and require live API access.

## Additional Documentation

- `docs/README.md`
- `docs/USAGE_PATTERNS.md`
- `docs/ADDITIONAL_HEADERS_EXAMPLES.md`