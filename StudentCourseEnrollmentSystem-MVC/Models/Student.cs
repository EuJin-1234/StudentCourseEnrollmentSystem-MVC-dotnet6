namespace Student_Course_Enrollment_System.Models
{
    public class Student
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public bool IsEnrolled { get; set; }
        public string UserId { get; set; }

        // Navigation property for the enrolled courses
        public ICollection<Enrollment>? Enrollments { get; set; }
    }
}
