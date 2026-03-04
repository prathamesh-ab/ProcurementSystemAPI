using ProcurementSystem.API.Models;

namespace ProcurementSystem.API.Interfaces
{
    public interface IAuthRepository
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByUsernameAsync(string username);
        Task<int> CreateUserAsync(User user);
        Task UpdateLastLoginAsync(int userId);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> UsernameExistsAsync(string username);
    }
}
