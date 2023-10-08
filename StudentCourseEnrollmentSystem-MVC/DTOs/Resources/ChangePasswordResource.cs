//Used in AuthController

namespace StudentCourseEnrollmentSystem_MVC.DTOs.Resources
{
    public class ChangePasswordResource
    {
        public string Email { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}
