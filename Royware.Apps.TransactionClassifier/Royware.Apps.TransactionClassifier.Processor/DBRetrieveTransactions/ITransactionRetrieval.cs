using Royware.Apps.TransactionClassifier.Processor.Models;

namespace Royware.Apps.TransactionClassifier.Processor.DBRetrieveTransactions
{
    public interface ITransactionRetrieval
    {
        Task<List<Transaction>> RetrieveUnresolvedTransactions(int batchSize);
    }
}
