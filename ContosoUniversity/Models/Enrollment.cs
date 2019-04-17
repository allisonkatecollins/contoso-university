namespace ContosoUniversity.Models
{
    public enum Grade
    {
        A, B, C, D, F
    }

    public class Enrollment
    {
        //EnrollmentID is the primary key by default
        public int EnrollmentID { get; set; }

        //CourseID property is a FK - corresponding navigation property is Course
        public int CourseID { get; set; }

        //StudentID is a FK - corresponding navigation property is Student
        public int StudentID { get; set; }

        //Grade property is an enum - keyword used to declare an enumeration
        // -- a type that consists of a set of named constants called the enumerator list
        //Grade is nullable
        public Grade? Grade { get; set; }

        public Course Course { get; set; }

        //Enrollment entity is associated with one Student identity
        //as opposed to Student.Enrollments in Student.cs, which can hold multiple Enrollment entities
        public Student Student { get; set; }
    }
}