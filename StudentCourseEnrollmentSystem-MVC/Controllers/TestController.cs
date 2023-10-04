using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Student_Course_Enrollment_System.Models;

namespace Student_Course_Enrollment_System.Controllers
{
    [ApiController]
    public class TestController :ControllerBase
    {

        //public IActionResult GetList()
        //{
        //    return Ok(true);
        //}
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public TestController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }


        [Authorize(Policy = "RequireAdministratorRole")]
        [Route("api/Test/GetUserList")]
        [HttpPost]
        public IActionResult GetUserList()
        {
            var users = _userManager.Users.Select(u => new UserViewModel
            {
                UserID = u.Id,
                Email = u.Email,
                Roles = string.Join(", ", _userManager.GetRolesAsync(u).Result)
            }).ToList();

            return Ok(users);
        }
    }
}
