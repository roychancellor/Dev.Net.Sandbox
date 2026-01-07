using Microsoft.Extensions.Options;
using NLog;
using Royware.Apps.TransactionClassifier.Logging;
using Royware.Apps.TransactionClassifier.Processor.CSVReadRawTransactions;
using Royware.Apps.TransactionClassifier.Providers.ApplicationSettings;

namespace Royware.Apps.TransactionClassifier.Processor
{
    public class TransactionProcessor
    {
        private static readonly Logger _log = Loggers.Batch;
        private readonly IOptionsMonitor<AppSettings> _appSettings;
        private readonly ITransactionReader _transReader;

        public TransactionProcessor(IOptionsMonitor<AppSettings> appSettings
                                   ,ITransactionReader transReader)
        {
            _appSettings = appSettings;
            _transReader = transReader;
        }

        public async Task ProcessAsync()
        {
            _log.Info($"====> Starting Batch");

            // File names are of the form Domain_AccountType_AnythingElse.csv
            var transactionsToProcess = _transReader.LoadFromFile(_appSettings.CurrentValue.FullPathToTransactionsFile);

            _log.Info($"<==== Batch Complete");
            return;
        }
    }
}
