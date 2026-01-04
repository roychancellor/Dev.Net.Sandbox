using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B2GnowTakehomeExercise.DataAccess.DataObjects
{
    public class EmployeePhone : IDataObject
    {
        public long ID { get; set; }
        // TODO: Add attributes to specify required, etc.
        public int EmployeeID { get; set; }
        public string PhoneArea { get; set; }
        public string Phone { get; set; }
        public string PhoneExt { get; set; }

        public bool IsProcessable()
        {
            return !(string.IsNullOrEmpty(PhoneArea) || string.IsNullOrEmpty(Phone) || string.IsNullOrEmpty(PhoneExt));
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{nameof(PhoneArea)}: {PhoneArea}");
            sb.AppendLine($"{nameof(Phone)}: {Phone}");
            sb.AppendLine($"{nameof(PhoneExt)}: {PhoneExt}");
            return sb.ToString();
        }
    }
}
