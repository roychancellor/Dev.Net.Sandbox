using Microsoft.Extensions.Options;
using NLog;
using Royware.Apps.TransactionClassifier.Logging;
using Royware.Apps.TransactionClassifier.Providers;

namespace Royware.Apps.TransactionClassifier.Processor
{
    public class TransactionProcessor
    {
        private static readonly Logger _log = Loggers.Batch;
        private readonly IOptionsMonitor<AppSettings> _appSettings;

        public TransactionProcessor(IOptionsMonitor<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        public async Task ProcessAsync()
        {
            _log.Info($"====> Starting Batch");
            _log.Info($"<==== Batch Complete");
            return;
        }
    }
}
