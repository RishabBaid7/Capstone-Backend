using ConstructionManagement_Backend.Models;

namespace ConstructionManagement_Backend.Services
{
    public interface IUserService
    {
        Task<bool> RegisterUserAsync(UserRegisterRequest user);
        Task<AuthenticationResult> AuthenticateUserAsync(string username, string password);
        //Task<(string Token, string Role)> AuthenticateUserAsync(string username, string password);
        Task<List<User>> GetAllUsersAsync();
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(string id);
        Task GetUserByUsernameAsync(string username);
    }
}
