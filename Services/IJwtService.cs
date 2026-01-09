using System.Security.Claims;

namespace WebApplication1.Services;

public interface IJwtService
{
    string GenerateToken(string username, string role, int userId);
    ClaimsPrincipal? ValidateToken(string token);
}


