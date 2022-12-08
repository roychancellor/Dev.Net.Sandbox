namespace ContosoUniversity.Models
{
    public enum Grade
    {
        A, B, C, D, F
    }

    public class Enrollment
    {
        public int EnrollmentID { get; set; } // this will become the PK (EF uses classnameID format for the PK)
        public int CourseID { get; set; } // this will be a FK (EF uses the convention <navigation property name><primary key property name> or just <primary key property name> for FKs)
        public int StudentID { get; set; } // FK
        public Grade? Grade { get; set; }

        public virtual Course Course { get; set; } // This will be a FK to the Courses table
        public virtual Student Student { get; set; } // This will be a FK to the Students table
    }
}