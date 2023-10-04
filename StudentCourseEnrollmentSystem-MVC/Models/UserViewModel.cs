
//Used in UsersController
//To perform CRUD for registered users
namespace Student_Course_Enrollment_System.Models
{
    public class UserViewModel
    {
        public int? Id { get; set; } 
        public string? UserID { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Roles { get; set; }
    }
}
