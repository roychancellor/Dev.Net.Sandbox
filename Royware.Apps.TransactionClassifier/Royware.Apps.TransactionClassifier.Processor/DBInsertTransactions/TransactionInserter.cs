using Microsoft.Data.SqlClient;
using NLog;
using Royware.Apps.TransactionClassifier.Logging;
using Royware.Apps.TransactionClassifier.Processor.DBRepository;
using Royware.Apps.TransactionClassifier.Processor.Models;
using System.Transactions;

namespace Royware.Apps.TransactionClassifier.Processor.DBInsertTransactions
{
    public class TransactionInserter : ITransactionInsert
    {
        private static readonly Logger _log = Loggers.Batch;
        private readonly ITransactionRepository _repo;

        public TransactionInserter(ITransactionRepository repo)
        {
            _repo = repo;
        }

        public async Task<int> InsertAllTransactions(List<Models.Transaction> transactions)
        {
            if (transactions == null || transactions.Count == 0)
            {
                _log.Warn($"There are no transactions to insert");
                return 0;
            }
            int numInserted = 0;
            int numDupes = 0;
            foreach (var t in transactions)
            {
                try
                {
                    numInserted += await _repo.InsertSingleTransaction(t);
                }
                catch (SqlException ex) when (ex.Number == 2627) // Unique constraint violation
                {
                    // Duplicate, just skip
                    _log.Warn($"Duplicate transaction: {t.TransAsString()}");
                    numDupes++;
                    continue;
                }
                catch (Exception ex)
                {
                    _log.Error(ex, $"While inserting transactions to database");
                    throw;
                } 
            }
            return numInserted + numDupes;
        }
    }
}
