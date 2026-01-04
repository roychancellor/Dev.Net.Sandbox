using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B2GnowTakehomeExercise.DataAccess.DataObjects
{
    public class EmployeeAddress : IDataObject
    {
        public long ID { get; set; }
        public int EmployeeID { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }

        public bool IsProcessable()
        {
            return true;
        }
    }
}
