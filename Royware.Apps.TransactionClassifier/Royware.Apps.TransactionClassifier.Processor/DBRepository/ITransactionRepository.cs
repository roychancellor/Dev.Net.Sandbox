using Royware.Apps.TransactionClassifier.Processor.Models;

namespace Royware.Apps.TransactionClassifier.Processor.DBRepository
{
    public interface ITransactionRepository
    {
        Task<List<Transaction>> RetrieveTransactions(int batchSize);
        Task<int> InsertTransactions(List<Transaction> transactions);
        Task<int> InsertSingleTransaction(Transaction transaction);
    }
}
