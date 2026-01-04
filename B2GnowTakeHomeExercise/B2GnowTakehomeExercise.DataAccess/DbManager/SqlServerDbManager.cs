using System.Data.SqlClient;
using System.Data;

namespace B2GnowTakehomeExercise.DataAccess.DbManager
{
    public class SqlServerDbManager : ISqlServerDbManager
    {
        // This should be in a config file
        public string ConnStr { get; set; }

        public SqlServerDbManager()
        {
            ConnStr = "Server=localhost;Database=TechnicalExercise;Trusted_Connection=True;";
        }

        public IDbConnection Connection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        public bool ExecuteNonQuery(IDbConnection sqlConnection, IDbCommand sqlCommand, out int rowsAffected)
        {
            if (sqlConnection == null || sqlCommand == null)
            {
                rowsAffected = 0;
                return false;
            }

            using (sqlConnection)
            {
                using (sqlCommand)
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlConnection.Open();
                    rowsAffected = sqlCommand.ExecuteNonQuery();
                    sqlConnection.Close();
                }
            }
            return true;
        }

        public bool ExecuteReader<T>(IDbConnection sqlConnection, IDbCommand sqlCommand, out DataTable results)
        {
            if (sqlConnection == null || sqlCommand == null)
            {
                results = null;
                return false;
            }
            var dt = new DataTable();
            using (sqlConnection)
            {
                using (sqlCommand)
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlConnection.Open();
                    var reader = sqlCommand.ExecuteReader();
                    dt.Load(reader);
                }
            }
            results = dt;
            return true;
        }

        public bool ExecuteScalar(IDbConnection conn, IDbCommand cmd, out object returnValue)
        {
            if (cmd == null || cmd == null || cmd.Connection == null)
            {
                returnValue = null;
                return false;
            }
            try
            {
                using (conn)
                {
                    using (cmd)
                    {
                        conn.Open();
                        returnValue = cmd.ExecuteScalar();
                        conn.Close();
                    }
                }
            }
            catch (Exception)
            {
                returnValue = null;
                return false;
            }
            return true;
        }

    }
}