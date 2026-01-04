using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkMessageProcessorModule.DataAccess.DbManager
{
    public interface ISqlServerDbManager
    {
        IDbConnection Connection(string connectionString);
        bool ExecuteScalar(IDbConnection sqlConnection, IDbCommand sqlCommand, out object result);
        bool ExecuteNonQuery(IDbConnection sqlConnection, IDbCommand sqlCommand, out int rowsAffected);
        bool ExecuteReader<T>(IDbConnection sqlConnection, IDbCommand sqlCommand, out IQueryable<T> resultTable);
    }
}
