using DapperDemoAPI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using Dapper;

namespace DapperDemoAPI.DAL
{
    public class UspCustomerRepository : ICustomerRepository
    {
        private IDbConnection _db;

        // CREATE
        public Customer InsertCustomer(Customer ourCustomer)
        {
            try
            {
                using (_db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    int rowsAffected = _db.Execute("dbo.uspInsertCustomer",
                                                    new
                                                    {
                                                        ourCustomer.CustomerFirstName,
                                                        ourCustomer.CustomerLastName,
                                                        IsActive = true
                                                    },
                                                    commandType: CommandType.StoredProcedure
                                                  );

                    if (rowsAffected > 0)
                    {
                        return GetCustomers(1, "DESC").SingleOrDefault();
                    }
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // READ
        public List<Customer> GetCustomers(int amount = 1000, string sort = "ASC")
        {
            try
            {
                using (_db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("amount", amount);
                    parameters.Add("sort", sort);

                    // Can also define the parameters anonymously
                    var pars = new { amount = amount, sort = sort };

                    //var toReturn = _db.Query<Customer>("dbo.uspGetTopCustomersSorted", parameters, commandType: CommandType.StoredProcedure).ToList();
                    var toReturn = _db.Query<Customer>("dbo.uspGetTopCustomersSorted", pars, commandType: CommandType.StoredProcedure).ToList();
                    return toReturn;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Customer GetSingleCustomer(int customerId)
        {
            try
            {
                using (_db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    //var parameters = new DynamicParameters();
                    //parameters.Add("CustomerID", customerId);
                    return _db.Query<Customer>("dbo.uspGetCustomerById", new { CustomerID = customerId }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // UPDATE
        public Customer UpdateCustomer(Customer ourCustomer)
        {
            try
            {
                using (_db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    int rowsAffected = _db.Execute("dbo.uspUpdateCustomer", ourCustomer, commandType: CommandType.StoredProcedure);

                    if (rowsAffected > 0)
                    {
                        return GetSingleCustomer(ourCustomer.CustomerID);
                    }
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // DELETE
        public bool DeleteCustomer(int customerId)
        {
            try
            {
                using (_db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    int rowsAffected = _db.Execute(@"dbo.uspDeleteCustomer", new { CustomerID = customerId }, commandType: CommandType.StoredProcedure);

                    if (rowsAffected > 0)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}