using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using B2GnowTakehomeExercise.DataAccess.DataObjects;

namespace B2GnowTakehomeExercise.DataAccess.CommandBuilders
{
    public class GetEmployeeListCommandBuilder : ICommandBuilder
    {
        public SqlCommand Build<T>(IDbConnection conn, string commandText, T parameters)
        {
            if (conn == null || string.IsNullOrEmpty(commandText))
            {
                return null;
            }

            var employeeParam = parameters as Employee;
            var pHireYearFilter = new SqlParameter("@HireYearFilter", SqlDbType.Int)
            {
                Direction = ParameterDirection.Input,
                Value = employeeParam?.HireDate,
            };

            var cmd = new SqlCommand
            {
                CommandText = commandText,
                CommandType = CommandType.StoredProcedure,
                Connection = conn as SqlConnection,
            };
            cmd.Parameters.Add(pHireYearFilter);

            return cmd;
        }
    }
}
