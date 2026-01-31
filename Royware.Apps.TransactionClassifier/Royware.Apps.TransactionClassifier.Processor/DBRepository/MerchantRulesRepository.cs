using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Royware.Apps.TransactionClassifier.Processor.Models;
using Royware.Apps.TransactionClassifier.Providers.ApplicationSettings;
using System.Data;

namespace Royware.Apps.TransactionClassifier.Processor.DBRepository
{
    public class MerchantRulesRepository : IMerchantRulesRepository
    {
        private readonly IOptionsMonitor<AppSettings> _appSettings;

        public MerchantRulesRepository(IOptionsMonitor<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        public async Task<List<MerchantRule>> RetrieveAllMerchantRules()
        {
            IEnumerable<MerchantRule> toReturn;
            using (SqlConnection connection = new(_appSettings.CurrentValue.DbConnString))
            {
                toReturn = await connection.QueryAsync<MerchantRule>($"SELECT * FROM {_appSettings.CurrentValue.ViewGetAllMerchantRules}", CommandType.Text);
            }
            return [.. toReturn];
        }

        public async Task<int> InsertMerchantRules(List<MerchantRule> merchantRules)
        {
            var dt = new DataTable();
            dt.Columns.Add(nameof(MerchantRule.NormalizedMerchant), typeof(string));
            dt.Columns.Add(nameof(MerchantRule.Domain), typeof(string));
            dt.Columns.Add(nameof(MerchantRule.AccountType), typeof(string));
            dt.Columns.Add(nameof(MerchantRule.Category), typeof(string));
            dt.Columns.Add(nameof(MerchantRule.RequiredTerms), typeof(string));
            dt.Columns.Add(nameof(MerchantRule.ExcludedTerms), typeof(string));
            dt.Columns.Add(nameof(MerchantRule.Priority), typeof(int));
            dt.Columns.Add(nameof(MerchantRule.Confidence), typeof(double));
            dt.Columns.Add(nameof(MerchantRule.IsActive), typeof(byte));
            dt.Columns.Add(nameof(MerchantRule.CreatedAt), typeof(DateTime));

            foreach (var mr in merchantRules)
            {
                dt.Rows.Add(mr.NormalizedMerchant,
                            mr.Domain,
                            mr.AccountType,
                            mr.Category,
                            string.Join(',', mr.RequiredTerms),
                            string.Join(',', mr.ExcludedTerms),
                            mr.Priority,
                            mr.Confidence,
                            1,
                            DateTime.Now);
            }

            var tvp = new DynamicParameters();
            tvp.Add("@MerchantRules", dt.AsTableValuedParameter("dbo.MerchantRuleTableType"));

            using SqlConnection conn = new(_appSettings.CurrentValue.DbConnString);
            var numInserted = await conn.ExecuteScalarAsync<int>(_appSettings.CurrentValue.ProcInsertMerchantRules, tvp, commandType: CommandType.StoredProcedure);

            return numInserted;
        }
    }
}
