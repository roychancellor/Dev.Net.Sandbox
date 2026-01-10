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

        /*
         * [9] 2026-01-07 20:31:04.9991 ERROR While inserting transactions to database Microsoft.Data.SqlClient.SqlException (0x80131904):
         * Violation of UNIQUE KEY constraint 'UQ_Transactions_ExternalTransactionHash'. Cannot insert duplicate key in object 'dbo.Transactions'.
         * The duplicate key value is (0x05d2345b3e9e9052ba0fb8c538521009a5f58b005f43866a256d4b403749052d).
        
        TODO: REVERT TO INDIVIDUAL TRANSACTION INSERTIONS AND TRY-CATCH -> LOG ERROR AND MOVE ON

         */
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
