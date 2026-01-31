using Royware.Apps.TransactionClassifier.Processor.Models;

namespace Royware.Apps.TransactionClassifier.Processor.DBRepository
{
    public interface ITransactionRepository
    {
        Task<List<Transaction>> RetrieveUnresolvedTransactionsBatch(int batchSize);
        Task<int> InsertTransactions(List<Transaction> transactions);
        Task<int> InsertSingleTransaction(Transaction transaction);
        Task<int> UpdateBatchTransactions(List<Transaction> transactions);
    }
}
