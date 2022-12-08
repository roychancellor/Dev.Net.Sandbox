using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{
    public class Course
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // allows manual entry of the PK rather than letting the SQL server generate it
        public int CourseID { get; set; } // PK
        public string Title { get; set; }
        public int Credits { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; } // Navigation property - one course can have multiple enrollments
    }
}