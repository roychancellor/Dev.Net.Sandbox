using Microsoft.Extensions.DependencyInjection;
using NLog;
using Royware.Apps.TransactionClassifier.Logging;
using Royware.Apps.TransactionClassifier.Processor;
using Royware.Apps.TransactionClassifier.AppStartup;
using Royware.Apps.TransactionClassifier.Providers.ApplicationSettings;

namespace Royware.Apps.TransactionClassifier
{
    internal class Program
    {
        private static readonly Logger _appLog = Loggers.App;
        
        static async Task Main(string[] args)
        {
            try
            {
                Loggers.Validate();

                Startup.LogApplicationAction(_appLog, "Transactify", ApplicationActions.Starting);
                
                var services = new ServiceCollection();

                _appLog.Info($"Binding appsettings");
                AppSettingsProvider.Bind(services, "appsettings.json");

                _appLog.Info($"Configuring dependencies");
                Startup.ConfigureDependencies(services);

                _appLog.Info($"Building service provider");
                var serviceProvider = services.BuildServiceProvider();

                _appLog.Info($"Getting transaction processor service from DI container");
                var app = serviceProvider.GetRequiredService<TransactionProcessor>();

                _appLog.Info($"Starting transaction processor application");
                Console.WriteLine("Transaction Classifier! STARTING BATCH...");
                
                await app.ProcessAsync();

                Startup.LogApplicationAction(_appLog, "Transactify", ApplicationActions.Finished);
            }
            catch (Exception ex)
            {
                _appLog.Fatal($"Fatal exception thrown | MESSAGE: {ex.Message}");
                return;
            }
        }
    }
}
