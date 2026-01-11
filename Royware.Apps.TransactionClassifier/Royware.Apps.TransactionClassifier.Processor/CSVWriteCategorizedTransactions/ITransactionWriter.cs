using Royware.Apps.TransactionClassifier.Processor.Models;

namespace Royware.Apps.TransactionClassifier.Processor.CSVWriteCategorizedTransactions
{
    public interface ITransactionWriter
    {
        Task<int> ExportTransactionsToCsv(List<Transaction> transactions, string path, FileMetaData fileMeta);
    }
}
