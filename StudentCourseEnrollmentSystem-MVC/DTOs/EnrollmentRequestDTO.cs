using Student_Course_Enrollment_System.Models;

namespace Student_Course_Enrollment_System.DTOs
{
    public class EnrollmentRequestDTO
    {
        public int EnrollmentID { get; set; }
        public int StudentID { get; set; }
        //[ForeignKey("StudentID")]
        public int CourseID { get; set; }
        //[ForeignKey("CourseID")]
        public bool IsEnrolled { get; set; }
    }
}
