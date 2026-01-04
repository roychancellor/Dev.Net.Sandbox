using InboundIO.Dashboard.ServicesMock.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

var rootBuilder = WebApplication.CreateBuilder(args);
var config = rootBuilder.Configuration;

// Load ports from appsettings.json
var ports = config.GetSection("HostedPorts").Get<int[]>() ?? Array.Empty<int>();

var tasks = ports.Select(port =>
{
    var builder = WebApplication.CreateBuilder();
    builder.WebHost.UseUrls($"http://localhost:{port}");

    var app = builder.Build();

    var rnd = new Random();

    var jsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = null, // PascalCase all property names
    };
    jsonOptions.Converters.Add(new JsonStringEnumConverter());

    app.MapGet("/health", () =>
    {
        if (rnd.Next(0, 10) <= 3) // ~60% chance of 503
        {
            return Results.StatusCode(503);
        }

        var status = (HealthStatus)rnd.Next(0, 3);

        var report = new HealthReport
        {
            HealthStatus = status,
            IsHealthy = status == HealthStatus.Healthy,
            DependenciesHealth = new Dictionary<string, string>
            {
                { "Database", ((HealthStatus)rnd.Next(0, 3)).ToString() },
                { "Cache", ((HealthStatus)rnd.Next(0, 3)).ToString() },
                { "ExternalApi", ((HealthStatus)rnd.Next(0, 3)).ToString() }
            }
        };

        return Results.Json(report, jsonOptions);
    });

    return app.RunAsync();
}).ToArray();

await Task.WhenAll(tasks);
