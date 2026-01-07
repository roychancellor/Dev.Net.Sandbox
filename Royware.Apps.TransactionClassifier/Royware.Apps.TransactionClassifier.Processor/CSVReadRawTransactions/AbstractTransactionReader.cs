using NLog;
using Royware.Apps.TransactionClassifier.Logging;
using Royware.Apps.TransactionClassifier.Processor.Models;
using System.Text.RegularExpressions;

namespace Royware.Apps.TransactionClassifier.Processor.CSVReadRawTransactions
{
    public abstract class AbstractTransactionReader : ITransactionReader
    {
        private static readonly Logger _log = Loggers.Batch;
        private readonly IFileNameParser _fileNameParser;

        public static Logger Log => _log;

        protected AbstractTransactionReader(IFileNameParser fileNameParser)
        {
            _fileNameParser = fileNameParser;
        }

        public virtual List<Transaction> LoadFromFile(FileMetaData fileMetaData)
        {
            var toReturn = new List<Transaction>();
            if (fileMetaData is null)
            {
                _log.Fatal($"Passed in file meta data object is null - returning empty");
                return toReturn;
            }
            var fullPathToFile = fileMetaData?.FullPathToFile;
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

            var transactionsRaw = File.ReadAllLines(fullPathToFile)
                                      .ToList();
            int transRowId = 1;
            foreach (var rt in transactionsRaw)
            {
                var transaction = ParseLine(rt);
                transaction?.Domain = fileMetaData.Domain.ToString();
                transaction?.AccountType = fileMetaData.AccountType.ToString();
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

        public abstract Transaction ParseLine(string transaction);
    }

    enum TransParts
    {
        Date,
        Amount,
        Asterisk,
        CheckNumber,
        Description,
        Debit,
        Credit,
    }
}
