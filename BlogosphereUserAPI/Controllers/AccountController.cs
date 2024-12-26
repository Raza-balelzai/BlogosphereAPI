using BlogosphereUserAPI.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlogosphereUserAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IConfiguration configuration; // For JWT Secret

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> Register(RegisterUserDTO registerUserDTO)
        {
            try
            {
                var user = new IdentityUser
                {
                    Email = registerUserDTO.Email,
                    UserName = registerUserDTO.UserName
                };
                var result = await userManager.CreateAsync(user, registerUserDTO.Password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "User");
                    return Ok(new { message = "Registered successfully" });
                }

                return BadRequest(new
                {
                    message = string.Join(", ", result.Errors.Select(e => e.Description))
                });
            }
            catch
            {
                return StatusCode(500, new { message = "An error occurred during registration." });
            }
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn(SignInUserDTO signInUserDTO)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(signInUserDTO.UserEmail);
                if (user == null) return BadRequest(new { message = "Invalid credentials." });

                var checkPassword = await userManager.CheckPasswordAsync(user, signInUserDTO.UserPassword);
                if (!checkPassword) return BadRequest(new { message = "Invalid credentials." });

                // Generate JWT
                var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

                // Get user roles
                var userRoles = await userManager.GetRolesAsync(user);

                // Add role claims
                authClaims.AddRange(userRoles.Select(role => new Claim("role", role))); // Use "role" as the claim type

                // Add Admin claim
                var isAdmin = userRoles.Contains("Admin") || userRoles.Contains("SuperAdmin");
                authClaims.Add(new Claim("isAdmin", isAdmin.ToString().ToLower())); // Add as "true" or "false"

                // Add User claim
                var isUser = userRoles.Contains("User") || userRoles.Contains("User");
                authClaims.Add(new Claim("isUser", isUser.ToString().ToLower())); // Add as "true" or "false"

                // Add superAdmin claim
                var isSuperAdmin = userRoles.Contains("SuperAdmin");
                authClaims.Add(new Claim("superAdmin", isSuperAdmin.ToString().ToLower())); // Add as "true" or "false"

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: configuration["JWT:ValidIssuer"],
                    audience: configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            catch
            {
                return StatusCode(500, new { message = "An error occurred during login." });
            }
        }




        [HttpPost("SignOut")]
        public IActionResult SignOut()
        {
            // Stateless JWT does not require SignOut logic.
            return Ok(new { message = "Token-based logout is handled on the client side by removing the token." });
        }
    }
}
