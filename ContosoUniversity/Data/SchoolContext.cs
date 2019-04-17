using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Data
{
    public class SchoolContext : DbContext
    {
        public SchoolContext(DbContextOptions<SchoolContext> options) : base(options)
        {
        }

        //DbSet property for each entity set
        //an entity set corresponds to a database table
        //an entity corresponds to a row in the table
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        //Students is the only necessary entity here because it references Enrollment, which references Course
        public DbSet<Student> Students { get; set; }

        //override default EF behavior of pluralizing table names
        //--specify singular table names in the DbContext
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>().ToTable("Course");
            modelBuilder.Entity<Enrollment>().ToTable("Enrollment");
            modelBuilder.Entity<Student>().ToTable("Student");
        }
    }
}
