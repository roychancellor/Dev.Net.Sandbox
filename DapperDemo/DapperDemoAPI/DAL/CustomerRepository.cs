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
    public class CustomerRepository : ICustomerRepository
    {
        private IDbConnection _db;

        // CREATE
        public Customer InsertCustomer(Customer ourCustomer)
        {
            using (_db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                int rowsAffected = _db.Execute(@"INSERT Customer([CustomerFirstName],[CustomerLastName],[IsActive]) values (@CustomerFirstName, @CustomerLastName, @IsActive)",
                                                    new
                                                    {
                                                        CustomerFirstName = ourCustomer.CustomerFirstName,
                                                        CustomerLastName = ourCustomer.CustomerLastName,
                                                        IsActive = true
                                                    }
                                                  );

                if (rowsAffected > 0)
                {
                    return GetCustomers(1, "DESC").SingleOrDefault();
                }
                return null;
            }
        }

        // READ
        public List<Customer> GetCustomers(int amount = 1000, string sort = "ASC")
        {
            using (_db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var toReturn = _db.Query<Customer>("SELECT TOP " + amount + " [CustomerID],[CustomerFirstName],[CustomerLastName],[IsActive] FROM [Customer] ORDER BY CustomerID " + sort).ToList();
                return toReturn;
            }
        }

        public Customer GetSingleCustomer(int customerId)
        {
            using (_db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                return _db.Query<Customer>("SELECT[CustomerID],[CustomerFirstName],[CustomerLastName],[IsActive] FROM [Customer] WHERE CustomerID =@CustomerID",
                                                new { CustomerID = customerId })
                                                .SingleOrDefault();
            }
        }

        // UPDATE
        public Customer UpdateCustomer(Customer ourCustomer)
        {
            using (_db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                int rowsAffected = _db.Execute("UPDATE [Customer] SET [CustomerFirstName] = @CustomerFirstName ,[CustomerLastName] = @CustomerLastName, [IsActive] = @IsActive WHERE CustomerID = " + ourCustomer.CustomerID, ourCustomer);

                if (rowsAffected > 0)
                {
                    return GetSingleCustomer(ourCustomer.CustomerID);
                }
                return null;
            }
        }

        // DELETE
        public bool DeleteCustomer(int customerId)
        {
            using (_db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                int rowsAffected = _db.Execute(@"DELETE FROM [dbo].[Customer] WHERE CustomerID = @CustomerID", new { CustomerID = customerId });

                if (rowsAffected > 0)
                {
                    return true;
                }
                return false; 
            }
        }
    }
}