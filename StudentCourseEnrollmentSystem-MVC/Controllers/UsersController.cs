using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Student_Course_Enrollment_System.Models;
using System.Linq;
using System.Threading.Tasks;


//UsersController
//SuperAdmin able to view,edit and delete registered user details
//Removed the Create function
//Imo, the Create function is unecessary, new users can be directly created from the register option

namespace Student_Course_Enrollment_System.Controllers
{

    [Authorize(Roles = "SuperAdmin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Users
        public IActionResult Index()
        {
            var users = _userManager.Users.Select(u => new UserViewModel
            {
                UserID = u.Id,
                Email = u.Email,
                FirstName = u.Firstname,
                LastName = u.Lastname,
                Roles = string.Join(", ", _userManager.GetRolesAsync(u).Result)
            }).ToList();

            return View(users);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userViewModel = new UserViewModel
            {
                UserID = user.Id,
                Email = user.Email,
                FirstName = user.Firstname,
                LastName = user.Lastname,
                Roles = string.Join(", ", _userManager.GetRolesAsync(user).Result)
            };

            return View(userViewModel);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userViewModel = new UserViewModel
            {
                UserID = user.Id,
                Email = user.Email,
                FirstName = user.Firstname,
                LastName = user.Lastname,
                Roles = string.Join(", ", _userManager.GetRolesAsync(user).Result)
            };

            return View(userViewModel);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserViewModel userViewModel)
        {
            if (id != userViewModel.UserID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                user.Email = userViewModel.Email;
                user.UserName = userViewModel.Email;

                await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
                await _userManager.AddToRolesAsync(user, userViewModel.Roles.Split(','));

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(userViewModel);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userViewModel = new UserViewModel
            {
                UserID = user.Id,
                Email = user.Email,
                Roles = string.Join(", ", _userManager.GetRolesAsync(user).Result)
            };

            return View(userViewModel);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(id);
        }
    }
}
