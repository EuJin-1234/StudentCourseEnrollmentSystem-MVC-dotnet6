using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Student_Course_Enrollment_System.Models;
using Student_Course_Enrollment_System.Resources;
using System.Data;
using AutoMapper;
using Student_Course_Enrollment_System.Models.Auth;
using Student_Course_Enrollment_System.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


// Signup, login, logout, change password API 
//Implement with Swagger
//Implement using UserManager 


namespace Student_Course_Enrollment_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtSettings _jwtSettings;

        public AuthController(
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptionsSnapshot<JwtSettings> jwtSettings)
        {
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp(UserSignUpResource userSignUpResource)
        {
            //var user = new ApplicationUser
            //{
            //    UserName = userSignUpResource.Email,
            //    Email = userSignUpResource.Email,
            //    Firstname = userSignUpResource.FirstName,
            //    Lastname = userSignUpResource.LastName
            //};

            var user = _mapper.Map<UserSignUpResource, ApplicationUser>(userSignUpResource);

            var result = await _userManager.CreateAsync(user, userSignUpResource.Password);

            if (result.Succeeded)
            {
                //// Generate JWT token
                //var roles = await _userManager.GetRolesAsync(user);
                //var token = GenerateJwt(user, roles);

                //return Ok(token);
                return Created(string.Empty, string.Empty);
            }

            return BadRequest(result.Errors.First().Description);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> SignIn(UserLoginResource userLoginResource)
        {
            var user = await _userManager.FindByEmailAsync(userLoginResource.Email);
            //var user = _userManager.Users.SingleOrDefault(u => u.UserName == userLoginResource.Email);


            if (user is null)
            {
                return NotFound("User not found");
            }

            var userSignInResult = await _userManager.CheckPasswordAsync(user, userLoginResource.Password);

            if (userSignInResult)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles is null)
                {
                    return BadRequest("User roles not found");
                }
                return Ok(GenerateJwt(user, roles));
            }

            return BadRequest("Email or password incorrect.");
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return Ok();
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordResource changePasswordResource)
        {
            var user = await _userManager.FindByEmailAsync(changePasswordResource.Email);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var result = await _userManager.ChangePasswordAsync(user, changePasswordResource.CurrentPassword, changePasswordResource.NewPassword);

            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest(result.Errors.First().Description);
        }


        [HttpPost("Roles")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return BadRequest("Role name should be provided.");
            }

            var newRole = new IdentityRole
            {
                Name = roleName
            };

            var roleResult = await _roleManager.CreateAsync(newRole);

            if (roleResult.Succeeded)
            {
                return Ok();
            }

            return Problem(roleResult.Errors.First().Description, null, 500);
        }

        
        [HttpPost("User/{userEmail}/Role")]
        public async Task<IActionResult> AddUserToRole(string userEmail, [FromBody] string roleName)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.UserName == userEmail);

            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                return Ok();
            }

            return Problem(result.Errors.First().Description, null, 500);
        }

        private string GenerateJwt(ApplicationUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var roleClaims = roles.Select(r => new Claim(ClaimTypes.Role, r));
            claims.AddRange(roleClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_jwtSettings.ExpirationInDays));

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Issuer,
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

