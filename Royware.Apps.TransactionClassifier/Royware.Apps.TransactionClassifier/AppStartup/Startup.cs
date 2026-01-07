using Microsoft.Extensions.DependencyInjection;
using NLog;
using Royware.Apps.TransactionClassifier.Processor;
using Royware.Apps.TransactionClassifier.Processor.CSVReadRawTransactions;
using Royware.Apps.TransactionClassifier.Processor.Models;

namespace Royware.Apps.TransactionClassifier.AppStartup
{
    public static class Startup
    {
        public static void ConfigureDependencies(ServiceCollection services)
        {
            services.AddSingleton<TransactionProcessor>();
            services.AddSingleton<WellsFargoTransactionReader>();
            services.AddSingleton<CitiBankTransactionReader>();
            services.AddSingleton<IFileNameParser, FileNameParser>();
            // This acts a small factory for getting the correct transaction reader based on the resolved transaction source
            services.AddSingleton<Func<TransactionSources, ITransactionReader>>(sp => key =>
            {
                return key switch
                {
                    TransactionSources.WellsFargo => sp.GetRequiredService<WellsFargoTransactionReader>(),
                    TransactionSources.CitiBank => sp.GetRequiredService<CitiBankTransactionReader>(),
                    _ => throw new ArgumentException($"Unknown {nameof(ITransactionReader)} type: {key}")
                };
            });
        }

        public static void LogApplicationAction(Logger log, string appName, ApplicationActions action)
        {
            var appNameDecorated = $" {appName} Application {action} ";

            var appNameToLog = appNameDecorated.PadLeft(appNameDecorated.Length + 10, '*');
            appNameToLog = appNameToLog.PadRight(appNameToLog.Length + 10, '*');
            
            var bannerBorder = string.Empty.PadLeft(appNameDecorated.Length + 20, '*');
            
            log.Info(bannerBorder);
            log.Info(appNameToLog);
            log.Info(bannerBorder);
        }
    }

    public enum ApplicationActions
    {
        Starting = 0,
        Finished
    }
}
