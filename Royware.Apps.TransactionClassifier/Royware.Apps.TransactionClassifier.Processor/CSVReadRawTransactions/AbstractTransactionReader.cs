using NLog;
using Royware.Apps.TransactionClassifier.Logging;
using Royware.Apps.TransactionClassifier.Processor.Models;
using System.Text.RegularExpressions;

namespace Royware.Apps.TransactionClassifier.Processor.CSVReadRawTransactions
{
    public abstract class AbstractTransactionReader : ITransactionReader
    {
        private static readonly Logger _log = Loggers.Batch;

        public static Logger Log => _log;
        
        public virtual List<Transaction> LoadFromFile(string fullPathToFile)
        {
            var toReturn = new List<Transaction>();
            if (string.IsNullOrWhiteSpace(fullPathToFile))
            {
                _log.Fatal($"Passed in file path is null or empty - returning empty");
                return toReturn;
            }
            if (!File.Exists(fullPathToFile))
            {
                _log.Fatal($"Unable to find the file {fullPathToFile} - returning empty");
                return toReturn;
            }
            if (!TryParseFileName(fullPathToFile, out string domain, out string accountType))
            {
                Log.Fatal($"The file name is formatted improperly. Must be: Domain_Account Type_Anything else.csv");
                return toReturn;
            }

            var transactionsRaw = File.ReadAllLines(fullPathToFile)
                                      .ToList();
            int transRowId = 1;
            foreach (var rt in transactionsRaw)
            {
                var transaction = ParseLine(rt);
                transaction?.Domain = domain;
                transaction?.AccountType = accountType;
                if (transaction == null || !transaction.IsProcessable())
                {
                    _log.Error($"The transaction is not processable - skipping | ROW: {transRowId} | TRANS: {rt}");
                    transRowId++;
                    continue;
                }
                transaction.Normalize();
                transaction.SetHash();

                toReturn.Add(transaction);
                transRowId++;
            }
            return toReturn;
        }

        private static bool TryParseFileName(string fullPathToFile, out string domain, out string accountType)
        {
            var fileName = Path.GetFileName(fullPathToFile);
            var domainAccountType = fileName.Split('_');
            var isValidFileName = domainAccountType.Length >= 2;
            domain = "";
            accountType = "";
            if (isValidFileName)
            {
                domain = domainAccountType[0];
                accountType = domainAccountType[1];
            }
            return isValidFileName;
        }

        public abstract Transaction ParseLine(string transaction);
    }
}
