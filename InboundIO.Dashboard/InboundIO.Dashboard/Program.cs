using InboundIO.Dashboard.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppSettings>(builder.Configuration);

var jsonOptions = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
};
jsonOptions.Converters.Add(new JsonStringEnumConverter());

var app = builder.Build();

var appSettings = app.Services.GetRequiredService<IOptionsMonitor<AppSettings>>().CurrentValue;
var healthEndpoints = appSettings.HealthCheckEndpoints;

app.MapGet("/api/health-summary", async () =>
{
    using var httpClient = new HttpClient();

    var tasks = healthEndpoints.Select(async pair =>
    {
        var (appName, url) = (pair.Key, pair.Value);

        try
        {
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();

            var report = await JsonSerializer.DeserializeAsync<HealthReport>(stream, jsonOptions);

            return new KeyValuePair<string, object>(appName, new
            {
                status = report?.HealthStatus.ToString() ?? "Unknown",
                raw = report
            });
        }
        catch (HttpRequestException hrex)
        {
            var strStatus = "Unhealthy";
            if (hrex.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            {
                strStatus = "UNREACHABLE";
            }
            return new KeyValuePair<string, object>(appName, new
            {
                status = strStatus,
                raw = new
                {
                    error = hrex.Message
                }
            });
        }
        catch (Exception ex)
        {
            return new KeyValuePair<string, object>(appName, new
            {
                status = "Unhealthy",
                raw = new
                {
                    error = ex.Message
                }
            });
        }
    });

    var results = await Task.WhenAll(tasks);
    return Results.Json(results.ToDictionary(kvp => kvp.Key, kvp => kvp.Value), jsonOptions);
});

// Serve static files (frontend)
app.UseDefaultFiles();
app.UseStaticFiles();

app.Run();
