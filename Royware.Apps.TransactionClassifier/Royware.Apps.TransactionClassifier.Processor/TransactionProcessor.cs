using Microsoft.Extensions.Options;
using NLog;
using Royware.Apps.TransactionClassifier.Logging;
using Royware.Apps.TransactionClassifier.Processor.CSVReadRawTransactions;
using Royware.Apps.TransactionClassifier.Processor.CSVWriteCategorizedTransactions;
using Royware.Apps.TransactionClassifier.Processor.DBInsertMerchantRules;
using Royware.Apps.TransactionClassifier.Processor.DBInsertTransactions;
using Royware.Apps.TransactionClassifier.Processor.DBRetrieveCategories;
using Royware.Apps.TransactionClassifier.Processor.DBRetrieveMerchantRules;
using Royware.Apps.TransactionClassifier.Processor.DBRetrieveTransactions;
using Royware.Apps.TransactionClassifier.Processor.DBUpdateBatchTransactions;
using Royware.Apps.TransactionClassifier.Processor.LogicCompareTransactionsToRules;
using Royware.Apps.TransactionClassifier.Processor.LogicGenerateUnmatchedRules;
using Royware.Apps.TransactionClassifier.Processor.Models;
using Royware.Apps.TransactionClassifier.Providers.ApplicationSettings;
using System.Data;

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
        private readonly ICategoriesRetrieve _categoriesRetriever;
        private readonly IMerchantRulesRetrieve _rulesRetriever;
        private readonly ITransactionRetrieval _transRetriever;
        private readonly IMerchantRuleTransactionMatcher _rulesMatcher;
        private readonly IMerchantRulesGeneration _rulesGenerator;
        private readonly IMerchantRulesInsertion _rulesInserter;
        private readonly ITransactionUpdate _transUpdater;
        private readonly ITransactionWriter _transWriter;

        public TransactionProcessor(IOptionsMonitor<AppSettings> appSettings
                                   ,Func<TransactionSources, ITransactionReader> readerFactory
                                   ,IFileNameParser fileNameParser
                                   ,ITransactionInsert transInserter
                                   ,ICategoriesRetrieve categoriesRetriever
                                   ,IMerchantRulesRetrieve rulesRetriever
                                   ,ITransactionRetrieval transRetriever
                                   ,IMerchantRuleTransactionMatcher rulesMatcher
                                   ,IMerchantRulesGeneration rulesGenerator
                                   ,IMerchantRulesInsertion rulesInserter
                                   ,ITransactionUpdate transUpdater
                                   ,ITransactionWriter transWriter)
        {
            _appSettings = appSettings;
            _fileNameParser = fileNameParser;
            _readerFactory = readerFactory;
            _transInserter = transInserter;
            _categoriesRetriever = categoriesRetriever;
            _rulesRetriever = rulesRetriever;
            _transRetriever = transRetriever;
            _rulesMatcher = rulesMatcher;
            _rulesGenerator = rulesGenerator;
            _rulesInserter = rulesInserter;
            _transUpdater = transUpdater;
            _transWriter = transWriter;
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

            // RETRIEVE KNOWN CATEGORIES
            _log.Info($"Retrieving known transaction categories");
            var knownCategories = await _categoriesRetriever.RetrieveActiveCategories();
            _log.Info($"Categories retrieved | COUNT: {knownCategories.Count}");

            if (knownCategories.Count == 0)
            {
                _log.Warn($"There are no active categories in the database. Ending.");
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

            List<Transaction> toExport = [];
            while (true)
            {
                // RETRIEVE UNRESOLVED TRANSACTIONS
                _log.Info($"Getting next batch of {_appSettings.CurrentValue.BatchSize} unresolved transactions");
                var transBatch = await _transRetriever.RetrieveUnresolvedTransactions(_appSettings.CurrentValue.BatchSize);
                if (transBatch.Count == 0)
                {
                    _log.Info($"There are no more unresolved transactions to process. End of processing.");
                    break;
                }
                _log.Info($"Retrieved {transBatch.Count} unresolved transactions.");

                // FIND A MERCHANT RULE FOR EACH TRANSACTION IN THE BATCH
                foreach (var tx in transBatch)
                {
                    var matchedRule = _rulesMatcher.MatchTransactionToRule(tx, merchantRules);
                    if (matchedRule == default)
                    {
                        _log.Warn($"Unable to match a merchant rule to transaction | TRANS: {tx.TransAsString()}");
                        continue;
                    }
                    _traceLog.Trace($"Matched rule to transaction | TRANS: {tx.TransAsString()} | RULE ID: {matchedRule}");
                    tx.ApplyMerchantRule(matchedRule);
                }

                // Unmatched transactions → AI
                var uresolvedTrans = transBatch.Where(t => !t.IsResolved).ToList();
                if (uresolvedTrans.Count > 0)
                {
                    //var aiPayload = _rulesGenerator.PrepareAIPayload(uresolvedTrans, merchantRules.Select(r => r.Category).ToList());
                    //var candidateRules = _rulesGenerator.CallAIForCandidateRules(aiPayload);
                    List<MerchantRuleProposal> candidateRules = [];
                    try
                    {
                        candidateRules = await _rulesGenerator.GetMerchantRuleProposalsAsync(uresolvedTrans,
                                                                                             knownCategories,
                                                                                             new CancellationTokenSource().Token);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex, $"While making AI call. Ending.");
                        return;
                    }
                    if (candidateRules.Count == 0)
                    {
                        _log.Warn($"The AI call returned zero candidate rules. Moving to next batch.");
                        continue;
                    }

                    var confirmedRules = _rulesGenerator.HumanReview(candidateRules);

                    // TODO: Implement inactivate/insert logic from https://chatgpt.com/c/69573d80-9e84-832e-b608-1c0ce926494a
                    var numRulesInserted = _rulesInserter.InsertMerchantRules(confirmedRules);

                    // Re-apply newly confirmed rules
                    foreach (var tx in uresolvedTrans)
                    {
                        var matchedRule = _rulesMatcher.MatchTransactionToRule(tx, confirmedRules);
                        if (matchedRule == null)
                        {
                            _log.Warn($"After creating new merchant rules, still unable to match rule to transaction | TRANS: {tx.TransAsString()}");
                            continue;
                        }
                        tx.ApplyMerchantRule(matchedRule);
                    }

                    // UPDATE BATCH TRANSACTIONS IN DATABASE
                    _log.Info($"Updating transactions in database for current batch | EXPECTED: {transBatch.Count}");
                    var numUpdated = await _transUpdater.UpdateBatchTransactions(transBatch);
                    if (numUpdated != transBatch.Count)
                    {
                        _log.Error($"The number of transactions updated does not match expected. Ending. | ACTUAL: {numUpdated} | EXPECTED: {transBatch.Count}");
                        return;
                    }
                    _log.Info($"Batch transactions updated | ACTUAL: {numUpdated}");

                    // Update in-memory cache of rules
                    merchantRules.AddRange(confirmedRules);
                }

                toExport.AddRange(transBatch);
            }

            // Export resolved transactions to CSV for Excel
            _log.Info($"Exporting all resolved transactions to CSV file | LOADED: {transactionsToProcess.Count} | PROCESSED: {toExport.Count}");
            await _transWriter.ExportTransactionsToCsv(toExport, _appSettings.CurrentValue.FullPathToExportTransactions, fileMeta);
            _log.Info($"Transactions exported.");

            _log.Info($"<==== Batch Complete");
            return;
        }
    }
}
