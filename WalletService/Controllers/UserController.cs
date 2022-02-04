using Domain.Models.UserIdentityModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PostgresInfrastructure.Interfaces;

namespace WalletService.Controllers
{
    [ApiController]
    [Route("User")]
    public class UserController : Controller
    {
        public UserManager<IdentityUser> _userManager { get; set; }
        public SignInManager<IdentityUser> _signInManager { get; set; }

        public UserController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IUserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("/register")]
        public async Task<IActionResult> Register([FromBody] Credentials credentials)
        {
            var user = new IdentityUser { UserName = credentials.Username, Email = credentials.Username };
            var result = await _userManager.CreateAsync(user, credentials.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _signInManager.SignInAsync(user, false);
            return Ok();
        }

        [HttpPost("/login")]
        public async Task<IActionResult> Login([FromBody] Credentials credentials)
        {
            var result = await _userManager.FindByEmailAsync(credentials.Username);
            if (result == null)
            {
                return NotFound("User Not found");
            }
            await _signInManager.SignInAsync(result, false);
            return Ok();
        }

        [Authorize] //Only use after Login or Register which signed in the system
        [HttpPost("/logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        [Authorize] //Only use after Login or Register which signed in the system
        [HttpDelete("/delete")]
        public async Task<IActionResult> Delete([FromBody] UserRemove _user)
        {
            if (string.IsNullOrEmpty(_user.UserId))
            {
                return BadRequest("UserId cannot be null or empty");
            }
            IdentityUser user = _userManager.FindByIdAsync(_user.UserId).Result;
            if (user == null)
            {
                return NotFound(_user.UserId + " -> UserId not found");
            }
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }

    }
}
