using ConstructionManagement_Backend.Models;
using ConstructionManagement_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstructionManagement_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest user)
        {
            if (!await _userService.RegisterUserAsync(user))
                return BadRequest("Username already exists.");

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            AuthenticationResult details = await _userService.AuthenticateUserAsync(loginRequest.Username, loginRequest.Password);

            if (details == null)
                return Unauthorized("Invalid username or password.");

            // Extract token and role from the response
            var token = details.Token; // Assuming `Token` is directly accessible in the `details` object
            var role = details.UserDetails.Role; // Assuming `Role` is inside `UserDetails`
            var username = details.UserDetails.Username;
            var email = details.UserDetails.Email;
            var id = details.UserDetails.Id;

            return Ok(new { Token = token, Role = role, Username = username, Email = email, UserId = id });  // Return the response

        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers() => Ok(await _userService.GetAllUsersAsync());

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] User user)
        {
            if (id != user.Id)
                return BadRequest("User ID mismatch.");

            await _userService.UpdateUserAsync(user);
            return Ok("User updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            await _userService.DeleteUserAsync(id);
            return Ok("User deleted successfully.");
        }
    }
}
