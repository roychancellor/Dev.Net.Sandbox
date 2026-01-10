using Microsoft.Extensions.Options;
using NLog;
using Royware.Apps.TransactionClassifier.Logging;
using Royware.Apps.TransactionClassifier.Processor.CSVReadRawTransactions;
using Royware.Apps.TransactionClassifier.Processor.DBInsertTransactions;
using Royware.Apps.TransactionClassifier.Processor.DBRetrieveMerchantRules;
using Royware.Apps.TransactionClassifier.Processor.Models;
using Royware.Apps.TransactionClassifier.Providers.ApplicationSettings;

namespace Royware.Apps.TransactionClassifier.Processor
{
    public class TransactionProcessor
    {
        private static readonly Logger _log = Loggers.Batch;
        private static readonly Logger _traceLog = Loggers.BatchTrace;
        private readonly IOptionsMonitor<AppSettings> _appSettings;
        private ITransactionReader? _transReader;
        private readonly IFileNameParser _fileNameParser;
        private readonly Func<TransactionSources, ITransactionReader> _readerFactory;
        private readonly ITransactionInsert _transInserter;
        private readonly IMerchantRulesRetrieve _rulesRetriever;

        public TransactionProcessor(IOptionsMonitor<AppSettings> appSettings
                                   ,Func<TransactionSources, ITransactionReader> readerFactory
                                   ,IFileNameParser fileNameParser
                                   ,ITransactionInsert transInserter
                                   ,IMerchantRulesRetrieve rulesRetriever)
        {
            _appSettings = appSettings;
            _fileNameParser = fileNameParser;
            _readerFactory = readerFactory;
            _transInserter = transInserter;
            _rulesRetriever = rulesRetriever;
        }

        public async Task ProcessAsync()
        {
            _log.Info($"====> Starting Batch");

            // PARSE FILE NAME
            // File names are of the form Source_Domain_AccountType_AnythingElse.csv
            var fullPathToFile = _appSettings.CurrentValue.FullPathToTransactionsFile;
            _log.Info($"Parsing file name to get file metadata | {fullPathToFile}");
            if (!_fileNameParser.TryParseFileName(fullPathToFile, out FileMetaData fileMeta))
            {
                _log.Fatal($"The file name is formatted improperly. Must be: Source_Domain_Account Type_Anything else.csv | FILE: {fullPathToFile}");
                return;
            }
            fileMeta.FullPathToFile = fullPathToFile;

            // LOAD TRANSACTIONS FROM FILE
            _log.Info($"Loading transactions from file | SOURCE: {fileMeta.Source} | DOMAIN: {fileMeta.Domain} | ACCT: {fileMeta.AccountType}");
            _transReader = _readerFactory(fileMeta.Source);
            _traceLog.Trace($"Reader factory type: {_readerFactory.GetType().Name}");
            var transactionsToProcess = _transReader.LoadFromFile(fileMeta);

            // INSERT TRANSACTIONS INTO DATABASE
            _log.Info($"Inserting transactions into database");
            var numProcessed = await _transInserter.InsertAllTransactions(transactionsToProcess);
            _log.Info($"Transactions processed | EXPECTED: {transactionsToProcess.Count} | ACTUAL: {numProcessed}");
            if (transactionsToProcess.Count != numProcessed)
            {
                _log.Error($"!! NOT ALL TRANSACTIONS PROCESSED (INSERTED OR DUPES SKIPPED) !!! Terminating Batch");
                return;
            }

            // RETRIEVE MERCHANT RULES
            _log.Info($"Retrieving active merchant rules");
            var merchantRules = await _rulesRetriever.RetrieveActiveMerchantRules();
            _log.Info($"Rules retrieved | COUNT: {merchantRules.Count}");

            if (merchantRules.Count == 0)
            {
                _log.Warn($"There are no active merchant rules in the database. Proceeding to rule creation.");
            }


            _log.Info($"<==== Batch Complete");
            return;
        }
    }
}
