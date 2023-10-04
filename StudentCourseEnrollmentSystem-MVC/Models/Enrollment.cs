using System.ComponentModel.DataAnnotations.Schema;

namespace Student_Course_Enrollment_System.Models
{
    public class Enrollment
    {
        public int EnrollmentID{get; set;}
        public int StudentID { get; set;}
        //[ForeignKey("StudentID")]
        public int CourseID { get; set;}
        //[ForeignKey("CourseID")]
        public bool IsEnrolled { get; set; }

        public bool IsRejected { get; set; }

        public bool IsWithdrawn { get; set; }
        // Navigation properties for the student and course
        public Student? Student { get; set; }
        public Course? Course { get; set; }
    }
}
