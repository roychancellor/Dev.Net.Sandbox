using LogSearchApp.DataContracts;
using LogSearchApp.SearchServices;
using NLog;
using NLog.Web;

var _appLog = LogManager.GetLogger(LoggerType.Application.ToString());
var _msLog = LogManager.GetCurrentClassLogger();

try
{
    _appLog.Info($"=====> STARTING APPLICATION CREATION PROCESS");
    var builder = WebApplication.CreateBuilder(args);

    _appLog.Info($"Configuring to use NLog");
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    _appLog.Info($"Getting app settings and binding to {nameof(LogSearchSettings)}");
    builder.Services.Configure<LogSearchSettings>(builder.Configuration.GetSection(nameof(LogSearchSettings)));

    _appLog.Info($"Adding controllers with views and adding services to DI container | {nameof(LogSearchService)}");
    builder.Services.AddControllersWithViews();
    builder.Services.AddSingleton<LogSearchService>();

    // Serve static files from wwwroot folder
    _appLog.Info($"Adding Razor pages to serve static files from the wwwroot folder");
    builder.Services.AddRazorPages();

    _appLog.Info($"Building application");
    var app = builder.Build();

    // Serve index.html from wwwroot
    _appLog.Info($"Configuring app to use default files (index.html) and to use static files");
    app.UseDefaultFiles();  // looks for index.html
    app.UseStaticFiles();

    // API endpoints
    _appLog.Info($"Configuring API endpoints");
    var searchEndpoint = "/api/logs/search";
    _appLog.Info($"Search Endpoint: {searchEndpoint}");
    app.MapPost(searchEndpoint, async (LogSearchService searchService, HttpContext http, LogSearchRequest request) =>
    {
        var result = await searchService.SearchAsync(request);
        return Results.Ok(result);
    });

    var searchByIdEndpoint = "/api/logs/log/{id}";
    _appLog.Info($"Search By ID Endpoint: {searchByIdEndpoint}");
    app.MapGet("/api/logs/log/{id}", async (LogSearchService searchService, string id) =>
    {
        var entry = await searchService.GetEntryByIdAsync(id);
        return entry is not null ? Results.Ok(entry) : Results.NotFound();
    });

    _appLog.Info($"<===== RUNNING APPLICATION");
    app.Run();
}
catch (Exception ex)
{
    _appLog.Error(ex, "<===== Application stopped due to exception during startup.");
    throw;
}
finally
{
    LogManager.Shutdown();
}
