using Tudormobile.USGS.Service;

namespace WebApiHost;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddUSGSClient(builder.Configuration);
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseUSGSService();

        // Run the application
        app.Run();
    }
}
