# Documentation
A full source-generated documentation website is not currently provided. This folder contains a collection of notes and documentatio files that will eventually form a complete comprehensive site.

## Development Notes
The USGS monitors groundwater locations in Clifton Park under specific station IDs, such as Sa-1285 (Site ID: 425048073472501) and Sa-1100 (Site ID: 425242073473201). You can target individual sites or fetch all groundwater sites in Saratoga County using the API endpoints detailed below.

curl -G "https://api.waterdata.usgs.gov/ogcapi/v0/collections/field-measurements/items" --data-urlencode "monitoring_location_id=USGS-425048073472501" --data-urlencode "datetime=2025-01-01/2026-05-29"   --data-urlencode "limit=1000"

Local number, Sa-1285, Clifton Park NY
Latitude 	42.8463888888889
Longitude 	-73.7908055555556 

API:

https://api.waterdata.usgs.gov/

Continuous values API documentation

https://api.waterdata.usgs.gov/ogcapi/v0/openapi?f=html#/continuous

Parameter
30210 	Water level, depth LSD 	m 	PHY 	Depth to water level, below land surface datum (LSD), meters 	Water 	None 	None

Obtain an API Key:

You can provide your API key either to the api_key query parameter or the X-Api-Key header. More information can be found in the API documentation.

servers: https://api.waterdata.usgs.gov/ogcapi/v0/

Curl command:

curl -H "Content-Type: application/json" -H "x-api-key: %USGS_API_KEY%" -G "https://api.waterdata.usgs.gov/ogcapi/v0/collections/daily/items" --data-urlencode "monitoring_location_id=USGS-425048073472501" --data-urlencode "datetime=2026-05-01/2026-05-30"   --data-urlencode "limit=1000"

This returns GeoJSON values. Maybe something less verbose?

Data Types Available:
 --data-urlencode "f=json"
 --data-urlencode "f=csv"
 --data-urlencode "f=html"
 --data-urlencode "f=jsonld"
