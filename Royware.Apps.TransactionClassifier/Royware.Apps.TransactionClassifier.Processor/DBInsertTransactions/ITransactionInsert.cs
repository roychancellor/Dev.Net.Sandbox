using Royware.Apps.TransactionClassifier.Processor.Models;

namespace Royware.Apps.TransactionClassifier.Processor.DBInsertTransactions
{
    public interface ITransactionInsert
    {
        Task<int> InsertAllTransactions(List<Transaction> transactions);
    }
}
