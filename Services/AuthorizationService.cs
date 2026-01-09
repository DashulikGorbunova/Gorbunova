using System.Security.Claims;

namespace WebApplication1.Services;

public interface IAuthorizationService
{
    bool CanRead(ClaimsPrincipal user);
    bool CanCreate(ClaimsPrincipal user);
    bool CanUpdate(ClaimsPrincipal user);
    bool CanDelete(ClaimsPrincipal user);
    string? GetUserRole(ClaimsPrincipal user);
    int? GetUserId(ClaimsPrincipal user);
}

public class AuthorizationService : IAuthorizationService
{
    private const string AdminRole = "Admin";
    private const string ManagerRole = "Manager";
    private const string UserRole = "User";

    public bool CanRead(ClaimsPrincipal user)
    {
        // Все роли могут читать
        return user.Identity?.IsAuthenticated == true || IsApiKey(user);
    }

    public bool CanCreate(ClaimsPrincipal user)
    {
        var role = GetUserRole(user);
        return role == AdminRole || role == ManagerRole || IsApiKey(user);
    }

    public bool CanUpdate(ClaimsPrincipal user)
    {
        var role = GetUserRole(user);
        return role == AdminRole || role == ManagerRole || IsApiKey(user);
    }

    public bool CanDelete(ClaimsPrincipal user)
    {
        var role = GetUserRole(user);
        return role == AdminRole || IsApiKey(user);
    }

    public string? GetUserRole(ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Role)?.Value 
            ?? user.FindFirst("role")?.Value;
    }

    public int? GetUserId(ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? user.FindFirst("userId")?.Value;
        
        if (int.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }
        
        return null;
    }

    private bool IsApiKey(ClaimsPrincipal user)
    {
        return user.HasClaim("ApiKey", _ => true);
    }
}

