using Microsoft.Extensions.DependencyInjection;
using NLog;
using Royware.Apps.TransactionClassifier.Processor;

namespace Royware.Apps.TransactionClassifier.AppStartup
{
    public static class Startup
    {
        public static void ConfigureDependencies(ServiceCollection services)
        {
            services.AddSingleton<TransactionProcessor>();
            //services.AddSingleton<OpenAiClientWrapper>();
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
