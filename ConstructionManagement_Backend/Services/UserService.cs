using ConstructionManagement_Backend.Models;
using ConstructionManagement_Backend.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ConstructionManagement_Backend.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<bool> RegisterUserAsync(UserRegisterRequest userRequest)
        {
            if (await _userRepository.GetUserByUsernameAsync(userRequest.Username) != null)
                return false;
            var user = new User();
            user.Username = userRequest.Username;
            user.Email = userRequest.Email;
            user.Role = userRequest.Role;
            user.PhoneNumber = userRequest.PhoneNumber;
            user.IsActive = userRequest.IsActive;
            //user.Id = Guid.NewGuid().ToString();
            user.Password = BCrypt.Net.BCrypt.HashPassword(userRequest.Password);
            await _userRepository.CreateUserAsync(user);
            return true;
        }

        public async Task<AuthenticationResult> AuthenticateUserAsync(string username, string password)
        {
            // Fetch user by username
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                return null;

            // Generate JWT token after successful authentication
            var token = GenerateJwtToken(user);

            // Return user details along with the token
            return new AuthenticationResult
            {
                Token = token,
                UserDetails = new User
                {
                    Id = user.Id,
                    Username = user.Username,
                    Role = user.Role,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    IsActive = user.IsActive
                }
            };
        }


        //public async Task<(string Token, string Role)> AuthenticateUserAsync(string username, string password)
        //{
        //    // Authenticate the user
        //    var user = await _userRepository.GetUserByCredentialsAsync(username, password);
        //    if (user == null)
        //        return (null, null);

        //    // Generate the token
        //    var token = GenerateJwtToken(user);

        //    // Return both the token and role
        //    return (token, user.Role);
        //}


        public async Task<List<User>> GetAllUsersAsync() => await _userRepository.GetAllUsersAsync();

        public async Task UpdateUserAsync(User user) => await _userRepository.UpdateUserAsync(user);

        public async Task DeleteUserAsync(string id) => await _userRepository.DeleteUserAsync(id);

        // Generate JWT token
        private string GenerateJwtToken(User user)
        {
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public Task GetUserByUsernameAsync(string username)
        {
            throw new NotImplementedException();
        }
    }
}
