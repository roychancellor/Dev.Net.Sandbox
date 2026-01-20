using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Royware.Apps.TransactionClassifier.Processor.Models;
using Royware.Apps.TransactionClassifier.Providers.ApplicationSettings;
using System.Data;

namespace Royware.Apps.TransactionClassifier.Processor.DBRepository
{
    public class CategoriesRepository : ICategoriesRepository
    {
        private readonly IOptionsMonitor<AppSettings> _appSettings;

        public CategoriesRepository(IOptionsMonitor<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        public async Task<List<Category>> RetrieveActiveCategories()
        {
            IEnumerable<Category> toReturn;
            using (SqlConnection connection = new(_appSettings.CurrentValue.DbConnString))
            {
                toReturn = await connection.QueryAsync<Category>($"SELECT * FROM {_appSettings.CurrentValue.ViewGetAllCategories}", CommandType.Text);
            }
            return [.. toReturn];
        }
    }
}
