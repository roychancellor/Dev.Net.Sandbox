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
    public class EmployeeDataAccessor : IDataAccessor<Employee>
    {
        private ICommandBuilder _empListCommandBuilder;
        private ICommandBuilder _empByIdCommandBuilder;
        private ICommandBuilder _insertEmpCommandBuilder;
        private string _procGetAll;
        private string _procGetById;
        private string _procInsertEmpPhone;

        public EmployeeDataAccessor()
        {
            _empListCommandBuilder = new GetEmployeeListCommandBuilder();
            _empByIdCommandBuilder = new GetEmployeeCommandBuilder();
            _insertEmpCommandBuilder = new InsertEmployeeCommandBuilder();
            _procGetAll = "dbo.proc_Employee_get_employee_list";
            _procGetById = "dbo.proc_Employee_get_employee";
            _procInsertEmpPhone = "dbo.proc_Employee_insert_phone";
        }

        public IEnumerable<Employee> GetAll()
        {
            try
            {
                var sqlMgr = SingletonSqlServerDbManager.Instance;
                var conn = sqlMgr.Connection(sqlMgr.ConnStr);
                var cmd = _empListCommandBuilder.Build(conn, _procGetAll, default(Employee));
                if (cmd == null)
                {
                    return null;
                }
                var isSuccessful = sqlMgr.ExecuteReader<Employee>(conn, cmd, out DataTable employeesDT);
                if (employeesDT == null)
                {
                    throw new Exception("GetAll: DataTable is null after call to database");
                }
                var toReturn = new List<Employee>();
                foreach (DataRow emp in employeesDT.Rows)
                {
                    toReturn.Add(new Employee
                    {
                        ID = long.Parse(emp["EmployeeID"].ToString()),
                        FirstName = emp["FirstName"].ToString(),
                        LastName = emp["LastName"].ToString(),
                        DateOfBirth = DateTime.Parse(emp["DateOfBirth"].ToString()),
                        HireDate = DateTime.Parse(emp["HireDate"].ToString()),
                    });
                }
                return toReturn;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Employee GetById(int id)
        {
            try
            {
                var sqlMgr = SingletonSqlServerDbManager.Instance;
                var conn = sqlMgr.Connection(sqlMgr.ConnStr);
                var empParam = new Employee { ID = id };
                var cmd = _empByIdCommandBuilder.Build(conn, _procGetById, empParam);
                if (cmd == null)
                {
                    return null;
                }
                var isSuccessful = sqlMgr.ExecuteReader<Employee>(conn, cmd, out DataTable employee);
                if (employee == null)
                {
                    throw new Exception("GetById: DataTable is null or empty after call to database");
                }
                if (employee.Rows.Count == 0)
                {
                    return new Employee { ID = -1 };
                }
                // TODO: Make into private method to be more DRY
                var emp = employee.Rows[0];
                var toReturn = new Employee
                {
                    ID = long.Parse(emp["EmployeeID"].ToString()),
                    FirstName = emp["FirstName"].ToString(),
                    LastName = emp["LastName"].ToString(),
                    DateOfBirth = DateTime.Parse(emp["DateOfBirth"].ToString()),
                    HireDate = DateTime.Parse(emp["HireDate"].ToString()),
                };
                return toReturn;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public InsertResult Insert(Employee toInsert)
        {
            throw new NotImplementedException();
        }
    }
}
