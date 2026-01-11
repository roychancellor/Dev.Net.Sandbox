using Royware.Apps.TransactionClassifier.Processor.Models;

namespace Royware.Apps.TransactionClassifier.Processor.CSVWriteCategorizedTransactions
{
    public class CSVTransactionWriter : ITransactionWriter
    {
        public Task<int> ExportTransactionsToCsv(List<Transaction> transactions, string path, FileMetaData fileMeta)
        {
            /*
             * Source_Domain_AccounType_...._DateTime.Now.csv
             * {0}_{1}_{2}_ResolvedTransactions_{3}.csv
             */
            throw new NotImplementedException();
        }
    }
}
