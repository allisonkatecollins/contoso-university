using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{
    public class Course
    {
        //DatabaseGenerated attribute lets you enter the PK for the course rather than having the DB generate it
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CourseID { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }

        //Enrollments is a navigation property
        //a Course entity can be related to any number of Enrollment entities
        public ICollection<Enrollment> Enrollments { get; set; }
    }
}