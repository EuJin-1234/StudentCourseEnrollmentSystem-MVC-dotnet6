using Microsoft.AspNetCore.Identity;

//Customize the register API to add the options of Firstname and Lastname

namespace Student_Course_Enrollment_System.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }
}
