using NLog;
using Royware.Apps.TransactionClassifier.Logging;
using Royware.Apps.TransactionClassifier.Processor.DBRepository;
using Royware.Apps.TransactionClassifier.Processor.Models;

namespace Royware.Apps.TransactionClassifier.Processor.DBUpdateBatchTransactions
{
    public class TransactionUpdater : ITransactionUpdate
    {
        private static readonly Logger _log = Loggers.Batch;
        private readonly ITransactionRepository _repo;

        public TransactionUpdater(ITransactionRepository repo)
        {
            _repo = repo;
        }

        public async Task<int> UpdateBatchTransactions(List<Transaction> transactions)
        {
            if (transactions == null || transactions.Count == 0)
            {
                _log.Error($"Passed in batch transactions list to update is null or empty");
                return 0;
            }

            try
            {
                _log.Info($"Updating batch transactions | EXPECTED: {transactions.Count}");
                var numUpdated = await _repo.UpdateBatchTransactions(transactions);
                _log.Info($"Transactions updated | ACTUAL: {numUpdated}");

                return numUpdated;
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"While updating batch transactions");
                throw;
            }
        }
    }
}
