//Used in AuthController
namespace Student_Course_Enrollment_System.Resources
{
    public class ChangePasswordResource
    {
        public string Email { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}
