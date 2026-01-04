using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Royware.Apps.TransactionClassifier.Providers
{
    public class AppSettingsProvider
    {
        public static void Bind(ServiceCollection services, string configFile = "appsettings.json")
        {
            var configuration = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile(configFile, optional: false, reloadOnChange: true)
                                .AddEnvironmentVariables()
                                .Build()
                                ?? throw new InvalidOperationException($"Unable to bind configuration from appsettings.json");
            
            var configSection = configuration!.GetSection("AppSettings");

            services.Configure<AppSettings>(configSection);
        }
    }
}
