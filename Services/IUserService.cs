using WebApplication1.Models.Entities;

namespace WebApplication1.Services;

public interface IUserService
{
    Task<User?> GetUserByUsernameAsync(string username);
    Task<User?> GetUserByIdAsync(int id);
    Task<User> CreateUserAsync(string username, string email, string password, string role);
    Task<bool> ValidatePasswordAsync(string password, string passwordHash);
    string HashPassword(string password);
}

