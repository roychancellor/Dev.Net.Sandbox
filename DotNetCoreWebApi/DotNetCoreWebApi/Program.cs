using DotNetCoreWebApi.Logic;
using DotNetCoreWebApi.Metrics;
using DotNetCoreWebApi.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using NLog;
using NLog.Web;

namespace DotNetCoreWebApi
{
    public class Program
    {
        private const string APP_SETTINGS_SECTION = "AppSettings";
        
        public static void Main(string[] args)
        {
            string msg;
            var _appLog = LogManager.GetLogger("AppLogger");
            _appLog.Info("=====> APPLICATION STARTING");

            _appLog.Trace("Creating web application builder");
            var builder = WebApplication.CreateBuilder(args);

            _appLog.Trace("Setting NLog as default logger (but will still use NLog directly)");
            builder.Logging.ClearProviders();
            builder.Host.UseNLog();

            // Add services to the container.
            _appLog.Trace("Adding controllers");
            builder.Services.AddControllers()
                            .AddXmlSerializerFormatters(); // not used now, but if decide to add controller that accepts XML directly

            _appLog.Trace("Registering RequestLogic as a singleton with a HttpContextAccessor and HttpClient management");
            builder.Services.AddSingleton<IRequestLogic, RequestLogic>()
                            .AddHttpContextAccessor()
                            .AddHttpClient();

            _appLog.Trace($"Getting {APP_SETTINGS_SECTION} section from appsettings.json");
            var appSettingsSection = builder.Configuration.GetSection(APP_SETTINGS_SECTION);
            if (!appSettingsSection.Exists())
            {
                msg = $"Unable to get section from appsettings.json | Section: {APP_SETTINGS_SECTION}";
                _appLog.Fatal(msg);
                throw new Exception(msg);
            }
            builder.Services.Configure<AppSettings>(appSettingsSection);

            _appLog.Trace("Building the builder");
            var app = builder.Build();

            // Add the metrics middleware
            app.UseMiddleware<MetricsMiddleware>();

            // Configure the HTTP request pipeline.
            _appLog.Trace("Configuring the Http pipeline");
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            _appLog.Info("<===== APPLICATION RUNNING");
            app.Run();
        }
    }
}
