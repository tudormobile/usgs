using Tudormobile.GeoJSON;

namespace SimpleConsoleApp
{
    internal class Program
    {
        private const string ApiUrl = "https://api.waterdata.usgs.gov/ogcapi/v0";
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            var apiKey = Environment.GetEnvironmentVariable("USGS_API_KEY");
            using var client = new HttpClient() { Timeout = TimeSpan.FromSeconds(30) };
            client.DefaultRequestHeaders.Add("x-api-key", apiKey);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var locationId = "425048073472501";
            var parameterCode = "72019";
            var format = "json";
            var path = "collections/daily/items";

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["parameter_code"] = parameterCode;
            query["monitoring_location_id"] = $"USGS-{locationId}";
            query["f"] = format;
            query["datetime"] = "2026-04-01/2026-05-30";
            query["limit"] = "1000";

            var builder = new UriBuilder(ApiUrl)
            {
                Path = $"/ogcapi/v0/{path}",
                Query = query.ToString()
            };

            var uri = builder.Uri;
            Console.WriteLine(uri);

            var json = await client.GetStringAsync(uri);

            Console.WriteLine(json);

            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
            var doc = await GeoJSONDocument.ParseAsync(stream);
            Console.WriteLine($"Found {doc.Objects["numberReturned"]} items");
            Console.WriteLine($"Timestamp: {doc.Objects["timeStamp"]}\n\n");
            foreach (var feature in doc.FeatureCollection.Features)
            {
                Console.WriteLine($"{feature.Properties["time"]} {feature.Properties["value"]} ft");
            }

        }
    }
}
