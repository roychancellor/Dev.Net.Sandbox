using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPREventReadBulkInserter.Models
{
    public class Person
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime? BirthDate { get; set; }
        public string FavoriteMovie { get; set; }
        public int? FavoriteNumber { get; set; }
    }
}
