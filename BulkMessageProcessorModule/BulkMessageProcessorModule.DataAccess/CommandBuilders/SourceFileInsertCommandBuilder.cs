using BulkMessageProcessorModule.DataAccess.DataObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkMessageProcessorModule.DataAccess.CommandBuilders
{
    public class SourceFileInsertCommandBuilder : ICommandBuilder
    {
        public SourceFileInsertCommandBuilder() { }

        public SqlCommand Build<T>(IDbConnection conn, string commandText, T parameters)
        {
            if (conn == null || string.IsNullOrEmpty(commandText) || parameters == null)
            {
                return null;
            }

            var sourceFile = parameters as SourceFile;

            var pSourceId = new SqlParameter("@SourceId", SqlDbType.BigInt);
            var pFilename = new SqlParameter("@Filename", SqlDbType.VarChar);
            var pNumRows = new SqlParameter("@NumRows", SqlDbType.BigInt);
            
            pSourceId.Direction = ParameterDirection.Input;
            pFilename.Direction = ParameterDirection.Input;
            pNumRows.Direction = ParameterDirection.Input;
            
            pSourceId.Value = sourceFile.SourceId;
            pFilename.Value = sourceFile.Filename;
            pNumRows.Value = sourceFile.NumRows;
            
            var cmd = new SqlCommand
            {
                CommandText = commandText,
                CommandType = CommandType.StoredProcedure,
                Connection = conn as SqlConnection,
            };
            cmd.Parameters.Add(pSourceId);
            cmd.Parameters.Add(pFilename);
            cmd.Parameters.Add(pNumRows);

            return cmd;
        }
    }
}
