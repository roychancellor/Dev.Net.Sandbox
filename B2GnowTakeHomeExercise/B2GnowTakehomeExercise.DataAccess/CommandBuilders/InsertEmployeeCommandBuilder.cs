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
    public class InsertEmployeeCommandBuilder : ICommandBuilder
    {
        public SqlCommand Build<T>(IDbConnection conn, string commandText, T parameters)
        {
            if (conn == null || string.IsNullOrEmpty(commandText) || parameters == null)
            {
                return null;
            }

            var empPhoneParam = parameters as EmployeePhone;
            if (empPhoneParam == null)
            {
                return null;
            }
            var pEmployeeID = new SqlParameter("@EmployeeID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Input,
                Value = empPhoneParam.ID,
            };
            var phoneArea = new SqlParameter("@PhoneArea", SqlDbType.VarChar)
            {
                Direction = ParameterDirection.Input,
                Value = empPhoneParam.PhoneArea,
            };
            var pPhone = new SqlParameter("@Phone", SqlDbType.VarChar)
            {
                Direction = ParameterDirection.Input,
                Value = empPhoneParam.Phone,
            };
            var pPhoneExt = new SqlParameter("@PhoneExt", SqlDbType.VarChar)
            {
                Direction = ParameterDirection.Input,
                Value = empPhoneParam.PhoneExt,
            };

            var cmd = new SqlCommand
            {
                CommandText = commandText,
                CommandType = CommandType.StoredProcedure,
                Connection = conn as SqlConnection,
            };
            cmd.Parameters.Add(pEmployeeID);
            cmd.Parameters.Add(phoneArea);
            cmd.Parameters.Add(pPhone);
            cmd.Parameters.Add(pPhoneExt);

            return cmd;
        }
    }
}
