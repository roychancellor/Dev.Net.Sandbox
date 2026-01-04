using B2GnowTakehomeExercise.DataAccess.DataObjects;
using B2GnowTakehomeExercise.DataAccess.DbManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using B2GnowTakehomeExercise.DataAccess.CommandBuilders;
using System.Data;

namespace B2GnowTakehomeExercise.DataAccess.DataAccessors
{
    public class EmployeePhoneDataAccessor : IDataAccessor<EmployeePhone>
    {
        private ICommandBuilder _insertEmpCommandBuilder;
        private string _procInsertEmpPhone;

        public EmployeePhoneDataAccessor()
        {
            _insertEmpCommandBuilder = new InsertEmployeeCommandBuilder();
            _procInsertEmpPhone = "dbo.proc_Employee_insert_phone";
        }

        public IEnumerable<EmployeePhone> GetAll()
        {
            throw new NotImplementedException();
        }

        public EmployeePhone GetById(int id)
        {
            throw new NotImplementedException();
        }

        public InsertResult Insert(EmployeePhone toInsert)
        {
            if (toInsert == null || !toInsert.IsProcessable())
            {
                return new InsertResult { ErrorCode = 0, ReturnMessage = $"EmployeePhone is null or not processable\n{toInsert}" };
            }

            try
            {
                var sqlMgr = SingletonSqlServerDbManager.Instance;
                var conn = sqlMgr.Connection(sqlMgr.ConnStr);
                var cmd = _insertEmpCommandBuilder.Build(conn, _procInsertEmpPhone, toInsert);
                if (cmd == null)
                {
                    return new InsertResult { ErrorCode = 0, ReturnMessage = $"Error while building SqlCommand" };
                }
                var isSuccessful = sqlMgr.ExecuteReader<Employee>(conn, cmd, out DataTable results);
                if (results == null || results.Rows.Count == 0)
                {
                    throw new Exception("Insert: DataTable is null or empty after call to database");
                }
                var result = results.Rows[0];
                var errorCode = int.Parse(result["Error"].ToString());
                var returnMsg = result["ReturnMessage"].ToString();
                return new InsertResult { ErrorCode = errorCode, ReturnMessage = returnMsg };
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
