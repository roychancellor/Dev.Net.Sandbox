using NLog;
using Royware.Apps.TransactionClassifier.Logging;
using Royware.Apps.TransactionClassifier.Processor.DBRepository;
using Royware.Apps.TransactionClassifier.Processor.Models;

namespace Royware.Apps.TransactionClassifier.Processor.DBRetrieveTransactions
{
    public class TransactionRetriever : ITransactionRetrieval
    {
        private static readonly Logger _log = Loggers.Batch;
        private readonly ITransactionRepository _repo;

        public TransactionRetriever(ITransactionRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Transaction>> RetrieveUnresolvedTransactions(int batchSize)
        {
            if (batchSize <= 0)
            {
                _log.Error($"Passed-in batch size is zero - returning empty list");
                return [];
            }

            try
            {
                return await _repo.RetrieveUnresolvedTransactionsBatch(batchSize);
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"While retrieving unresolved transactions");
                return [];
            }
        }
    }
}
