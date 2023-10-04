
//Used in Students1Controller
//To create new student record

namespace Student_Course_Enrollment_System.Models
{
    
    public class CreateStudentViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<int>? SelectedCourseIds { get; set; }
        public List<Course>? Courses { get; set; }
    }
}
