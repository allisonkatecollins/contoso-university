using System;
using System.Collections.Generic;

namespace ContosoUniversity.Models
{
    public class Student
    {   
        //ID property is the primary key column of the corresponding DB table
        public int ID { get; set; }
        public string LastName { get; set; }
        public string FirstMidName { get; set; }
        public DateTime EnrollmentDate { get; set; }

        //Enrollments property is a navigation property - holds other entities that are related to this entity
        //-- so the Enrollments property of a Student entity will hold all of the Enrollment entities related to the Student entity
        // if a navigation property can hold multiple entities (i.e. many-to-many or one-to-many relationships,
        // -- its type must be a list in which entries can be added, deleted, and updated
        // -- hence why we are using the ICollection<> type here
        public ICollection<Enrollment> Enrollments { get; set; }
    }
}