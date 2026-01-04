using B2GnowTakehomeExercise.DataAccess.DataAccessors;
using B2GnowTakehomeExercise.DataAccess.DataObjects;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http.Results;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace B2GnowTakeHomeExercise.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        // GET: api/<EmployeesController>
        [HttpGet]
        public IEnumerable<Employee> Get()
        {
            // TODO: Implement hire date filter
            List<Employee> toReturn = new EmployeeDataAccessor().GetAll().ToList();
            return toReturn;
        }

        // GET api/<EmployeesController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            Employee toReturn = new EmployeeDataAccessor().GetById(id);
            if (toReturn.ID < 0)
            {
                return NotFound($"Employee Not Found with ID: " + id);
            }
            return Ok(toReturn);
        }

        // POST api/<EmployeesController>
        [HttpPost]
        public IActionResult Post([FromBody] EmployeePhone empPhone)
        {
            // TODO: Add model validation here and return 400 (bad request) if fails model validation
            
            // TODO: Modify the database schema to FK the EmployeeID in the EmployeePhone and EmployeeAddress tables
            // and update the stored procedure to check for existence of EmployeeID before trying to insert.

            var result = new EmployeePhoneDataAccessor().Insert(empPhone);
            if (result.ErrorCode != 0)
            {
                return StatusCode(500, result);
            }
            return Ok(result);
        }
    }
}
