using DapperDemoAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperDemoAPI.DAL
{
    public interface ICustomerRepository
    {
        Customer InsertCustomer(Customer ourCustomer);
        List<Customer> GetCustomers(int amount, string sort);
        Customer GetSingleCustomer(int customerId);
        Customer UpdateCustomer(Customer ourCustomer);
        bool DeleteCustomer(int customerId);
    }
}
