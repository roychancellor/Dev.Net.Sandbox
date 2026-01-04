using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperDemoAPI.Models
{
    public class Customer
    {
        public int CustomerID { get; set; }
        
        [Required(ErrorMessage = "Customer First Name is required")]
        [StringLength(50, ErrorMessage = "Customer First Name must be 50 characters or fewer")]
        public string CustomerFirstName { get; set; }
        
        [Required(ErrorMessage = "Customer Last Name is required")]
        [StringLength(50, ErrorMessage = "Customer Last Name must be 50 characters or fewer")]
        public string CustomerLastName { get; set; }
        
        public bool IsActive { get; set; }
    }
}
