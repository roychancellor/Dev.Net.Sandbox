using Microsoft.Extensions.Configuration;

namespace Royware.Apps.TransactionClassifier.Providers
{
    public class AppSettingsProvider
    {
        public static IConfiguration Bind(string configFile = "appsettings.json")
        {
            var configuration = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile(configFile, optional: false, reloadOnChange: true)
                                .AddEnvironmentVariables()
                                .Build();
            
            var configSection = configuration!.GetSection("AppSettings");

            return configSection;
        }
    }
}
