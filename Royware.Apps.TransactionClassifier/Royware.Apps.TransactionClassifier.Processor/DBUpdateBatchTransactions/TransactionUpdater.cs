using Royware.Apps.TransactionClassifier.Processor.Models;

namespace Royware.Apps.TransactionClassifier.Processor.DBUpdateBatchTransactions
{
    public class TransactionUpdater : ITransactionUpdate
    {
        public async Task<int> UpdateBatchTransactions(List<Transaction> transactions)
        {
            throw new NotImplementedException();
        }
    }
}
