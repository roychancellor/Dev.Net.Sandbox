using Microsoft.Extensions.Options;
using NLog;
using Royware.Apps.TransactionClassifier.Logging;
using Royware.Apps.TransactionClassifier.Processor.CSVReadRawTransactions;
using Royware.Apps.TransactionClassifier.Processor.Models;
using Royware.Apps.TransactionClassifier.Providers.ApplicationSettings;

namespace Royware.Apps.TransactionClassifier.Processor
{
    public class TransactionProcessor
    {
        private static readonly Logger _log = Loggers.Batch;
        private readonly IOptionsMonitor<AppSettings> _appSettings;
        private ITransactionReader? _transReader;
        private readonly IFileNameParser _fileNameParser;
        private readonly Func<TransactionSources, ITransactionReader> _readerFactory;

        public TransactionProcessor(IOptionsMonitor<AppSettings> appSettings
                                   ,Func<TransactionSources, ITransactionReader> readerFactory
                                   ,IFileNameParser fileNameParser)
        {
            _appSettings = appSettings;
            _fileNameParser = fileNameParser;
            _readerFactory = readerFactory;
        }

        public async Task ProcessAsync()
        {
            _log.Info($"====> Starting Batch");

            // File names are of the form Source_Domain_AccountType_AnythingElse.csv
            var fullPathToFile = _appSettings.CurrentValue.FullPathToTransactionsFile;
            if (!_fileNameParser.TryParseFileName(fullPathToFile, out FileMetaData fileMeta))
            {
                _log.Fatal($"The file name is formatted improperly. Must be: Source_Domain_Account Type_Anything else.csv | FILE: {fullPathToFile}");
                return;
            }
            fileMeta.FullPathToFile = fullPathToFile;

            _transReader = _readerFactory(fileMeta.Source);
            var transactionsToProcess = _transReader.LoadFromFile(fileMeta);

            _log.Info($"<==== Batch Complete");
            return;
        }
    }
}
