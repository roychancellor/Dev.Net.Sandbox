using Royware.Apps.TransactionClassifier.Processor.Models;

namespace Royware.Apps.TransactionClassifier.Processor.DBUpdateBatchTransactions
{
    public interface ITransactionUpdate
    {
        Task<int> UpdateBatchTransactions(List<Transaction> transactions);
    }
}
