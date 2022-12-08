using ContosoUniversity.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace ContosoUniversity.DAL
{
    public class SchoolContext : DbContext
    {
        public SchoolContext() : base("SchoolContextConnection") // the connection string will come from Web.config
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; } // This is strictly not necessary, as EF would still make the table because Students 'has a' Enrollments
        public DbSet<Course> Courses { get; set; } // Same idea for Courses

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>(); // prevents EF from pluralizing the DB tables it creates, e.g., 'Student' instead of 'Students'
        }
    }
}