using System.Linq;
using System.Threading.Tasks;
using PertixCore.Resources;
using PertixCore.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PertixCore.Api.Resources;
using PertixCore.Core.Models;

namespace PertixCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IJwtService _jwtService;
        private readonly IEmailSender _emailSender;
        private readonly IMapper _mapper;

        public AuthController(
            IMapper mapper,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<Role> roleManager,
            IJwtService jwtService,
            IEmailSender emailSender)
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
            _emailSender = emailSender;

        }

        [HttpPost("signUp")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp(UserSignUpResource userSignUpResource)
        {
            var user = _mapper.Map<UserSignUpResource, User>(userSignUpResource);

            var userCreateResult = await _userManager.CreateAsync(user, userSignUpResource.Password);

            if (userCreateResult.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action(nameof(ConfirmEmail), "Auth", new { token, email = user.Email }, Request.Scheme);
                
                var message = new Message(new string[] { user.Email }, "Confirmation email link", confirmationLink, null);
                await _emailSender.SendEmailAsync(message);

                return Created(string.Empty, string.Empty);
            }

            
            return Problem(userCreateResult.Errors.First().Description, null, 500);
        }

        [HttpGet("user/emailConfirm")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByNameAsync(email);
            if (user == null)
                return BadRequest("Error");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            return Ok(result.Succeeded ? "Email confirmation successful" : "An error ocurred while processing your request.");
        }

        [HttpPost("signIn")]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn(UserLoginResource userLoginResource)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.UserName == userLoginResource.Email);
            if (user is null)
                return NotFound(new { message = "User not found." });

            var userSigninResult = await _signInManager.PasswordSignInAsync(user, userLoginResource.Password, false, false);

            if (userSigninResult.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var response = new
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Token = _jwtService.GenerateJwt(user, roles)
                };

                return Ok(response);
            }

            var userIsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
            if(!userIsEmailConfirmed)
                return BadRequest(new { message = "Email is not confirmed." });

            return BadRequest(new { message = "Email or password is incorrect." });
        }

        [HttpPost("forgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordResource forgotPasswordResource)
        {

            var user = await _userManager.FindByEmailAsync(forgotPasswordResource.Email);
            if (user == null)
                return NotFound("User not found");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            /*var callback = Url.Action(nameof(ResetPassword), nameof(AuthController), new { token, email = user.Email }, Request.Scheme);

            var message = new Message(new string[] { user.Email }, "Reset password token", callback, null);
            await _emailSender.SendEmailAsync(message);*/

            return Ok(new
            {
                token = token,
                email = user.Email
            });
        }

        [HttpPost("resetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordResource resetPasswordResourcel)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordResourcel.Email);
            if (user == null)
                return BadRequest("Error");

            var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPasswordResourcel.Token, resetPasswordResourcel.NewPassword);
            if (resetPassResult.Succeeded)
            {
                return Ok(new { message = "Password has been reset" });
            }

            return BadRequest("Incorrect email or token");
        }

        [HttpPost("createRole")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return BadRequest("Role name should be provided.");
            }

            var newRole = new Role
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

        [HttpPost("user/{userEmail}/asignRole")]
        [Authorize(Roles = "admin")]
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

        [HttpPost("user/logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return Ok(new { message = "User logged out" });
        }
    }
}
