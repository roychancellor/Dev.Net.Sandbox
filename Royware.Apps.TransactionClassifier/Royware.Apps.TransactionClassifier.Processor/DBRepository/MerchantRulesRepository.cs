using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using NLog;
using Royware.Apps.TransactionClassifier.Logging;
using Royware.Apps.TransactionClassifier.Processor.Models;
using Royware.Apps.TransactionClassifier.Providers.ApplicationSettings;
using System.Data;

namespace Royware.Apps.TransactionClassifier.Processor.DBRepository
{
    public class MerchantRulesRepository : IMerchantRulesRepository
    {
        private static readonly Logger _log = Loggers.Batch;
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
    }
}
