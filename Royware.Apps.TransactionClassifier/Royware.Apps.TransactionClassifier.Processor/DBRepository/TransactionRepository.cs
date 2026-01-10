using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Royware.Apps.TransactionClassifier.Processor.Models;
using Royware.Apps.TransactionClassifier.Providers.ApplicationSettings;
using System.Data;

namespace Royware.Apps.TransactionClassifier.Processor.DBRepository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly IOptionsMonitor<AppSettings> _appSettings;

        public TransactionRepository(IOptionsMonitor<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        public async Task<List<Transaction>> RetrieveTransactions(int batchSize)
        {
            if (batchSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(batchSize), "The batch size must be greater than zero");
            }
            var toReturn = new List<Transaction>();

            using (var conn = new SqlConnection(_appSettings.CurrentValue.DbConnString))
            {
                var inputs = new DynamicParameters();
                inputs.Add("@BatchSize", _appSettings.CurrentValue.BatchSize);

                var transactions = await conn.QueryAsync<Transaction>(_appSettings.CurrentValue.ProcGetUnresolvedTransactions, inputs, commandType: CommandType.StoredProcedure);
                toReturn = transactions.Any() ? [.. toReturn] : toReturn;
            }

            return toReturn;
        }

        public async Task<int> InsertTransactions(List<Transaction> transactions)
        {
            /*
             * [Description] [nvarchar](500) NULL,
	[Amount] [decimal](18, 2) NULL,
	[TransactionDate] [date] NULL,
	[DomainName] [nvarchar](100) NULL,
	[AccountTypeName] [nvarchar](100) NULL,
	[ExternalTransactionHash] [binary](32) NULL
             */
            var transactionTable = new DataTable();
            transactionTable.Columns.Add("Description", typeof(string));
            transactionTable.Columns.Add("Amount", typeof(decimal));
            transactionTable.Columns.Add("TransactionDate", typeof(DateTime));
            transactionTable.Columns.Add("DomainName", typeof(string));
            transactionTable.Columns.Add("AccountTypeName", typeof(string));
            transactionTable.Columns.Add("ExternalTransactionHash", typeof(byte[]));

            foreach (var t in transactions)
            {
                transactionTable.Rows.Add(t.Description, t.Amount, t.TransactionDate, t.Domain, t.AccountType, t.ExternalTransactionHash);
            }

            int rowsInserted;
            using (var connection = new SqlConnection(_appSettings.CurrentValue.DbConnString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Transactions", transactionTable.AsTableValuedParameter("dbo.TransactionTableType"));

                rowsInserted = await connection.ExecuteAsync(_appSettings.CurrentValue.ProcInsertMultipleTransactions, parameters, commandType: CommandType.StoredProcedure);
            }

            return rowsInserted;
        }

        public async Task<int> InsertSingleTransaction(Transaction transaction)
        {
            /*
             * [Description] [nvarchar](500) NULL,
	[Amount] [decimal](18, 2) NULL,
	[TransactionDate] [date] NULL,
	[DomainName] [nvarchar](100) NULL,
	[AccountTypeName] [nvarchar](100) NULL,
	[ExternalTransactionHash] [binary](32) NULL
             */
            var parameters = new DynamicParameters();
            parameters.Add("@Description", transaction.Description);
            parameters.Add("@Amount", transaction.Amount);
            parameters.Add("@TransactionDate", transaction.TransactionDate);
            parameters.Add("@DomainName", transaction.Domain);
            parameters.Add("@AccountTypeName", transaction.AccountType);
            parameters.Add("@ExternalTransactionHash", transaction.ExternalTransactionHash);

            int rowsInserted;
            using (var connection = new SqlConnection(_appSettings.CurrentValue.DbConnString))
            {
                rowsInserted = await connection.ExecuteAsync(_appSettings.CurrentValue.ProcInsertSingleTransaction, parameters, commandType: CommandType.StoredProcedure);
            }

            return rowsInserted;
        }
    }
}
