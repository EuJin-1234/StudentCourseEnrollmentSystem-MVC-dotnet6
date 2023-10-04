namespace Student_Course_Enrollment_System.Models
{
    public class Course
    {
        public int courseId { get; set; }
        public string courseName { get; set; }

        // Navigation property for the enrolled students
        public ICollection<Enrollment>? Enrollments { get; set; }
    }
}
