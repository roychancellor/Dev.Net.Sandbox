using DapperDemoAPI.DAL;
using DapperDemoAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DapperDemoAPI.Controllers
{
    public class CustomerController : ApiController
    {
        private ICustomerRepository _ourCustomerRepository;

        public CustomerController()
        {
            //_ourCustomerRepository = new CustomerRepository();
            _ourCustomerRepository = new UspCustomerRepository();
        }

        // CREATE
        [Route("Customers")]
        [HttpPost]
        public IHttpActionResult Post([FromBody] Customer ourCustomer)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var customerToReturn = _ourCustomerRepository.InsertCustomer(ourCustomer);
                return Created("Customers", customerToReturn);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // READ
        [Route("Customers/{amount}/{sort}")]
        [HttpGet]
        public IHttpActionResult Get(int amount, string sort)
        {
            try
            {
                if (amount <= 0)
                {
                    return BadRequest("ERROR: amount must be a positive integer");
                }
                if (!(sort.ToUpper() == "ASC" || sort.ToUpper() == "DESC"))
                {
                    return BadRequest("ERROR: sort must be asc, ASC, desc, or DESC");
                }
                var customersToReturn = _ourCustomerRepository.GetCustomers(amount, sort);
                return Ok(customersToReturn);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("Customers/{id}")]
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("ERROR: id must be a positive integer!!!");
                }
                var customerToReturn = _ourCustomerRepository.GetSingleCustomer(id);
                return Ok(customerToReturn);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // UPDATE
        [Route("Customers")]
        [HttpPut]
        public IHttpActionResult Put([FromBody]Customer ourCustomer)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (ourCustomer.CustomerID <= 0)
                {
                    return BadRequest("ERROR: customer id must be a positive integer!!!");
                }
                var customerToReturn = _ourCustomerRepository.UpdateCustomer(ourCustomer);
                if (customerToReturn == null)
                {
                    return BadRequest($"Customer with ID {ourCustomer.CustomerID} was not found - nothing to update");
                }
                return Ok(customerToReturn);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // DELETE
        [Route("Customers/{id}")]
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("ERROR: id must be a positive integer!!!");
                }
                bool toReturn = _ourCustomerRepository.DeleteCustomer(id);
                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
