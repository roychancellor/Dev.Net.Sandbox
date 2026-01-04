using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using Royware.Apps.TransactionClassifier.Logging;
using Royware.Apps.TransactionClassifier.Processor;
using Royware.Apps.TransactionClassifier.Providers;

namespace Royware.Apps.TransactionClassifier
{
    internal class Program
    {
        private static readonly Logger _appLog = Loggers.App;
        
        static async Task Main(string[] args)
        {
            IConfiguration configuration;
            try
            {
                configuration = AppSettingsProvider.Bind("appsettings.json")
                              ?? throw new InvalidOperationException("AppSettings section is missing or invalid."); ;
            }
            catch (Exception ex)
            {
                _appLog.Fatal($"Unable to bind configuration from appsettings.json | MESSAGE: {ex.Message}");
                return;
            }
            
            var services = new ServiceCollection();

            services.Configure<AppSettings>(configuration);

            // App services
            services.AddSingleton<App>();
            services.AddSingleton<TransactionProcessor>();
            //services.AddSingleton<OpenAiClientWrapper>();

            var provider = services.BuildServiceProvider();

            var app = provider.GetRequiredService<App>();

            Console.WriteLine("Transaction Classifier! STARTING BATCH...");
            await app.RunAsync();
            Console.WriteLine("BATCH COMPLETE! Press any key to finish.");
            Console.ReadKey();
        }
    }
}
