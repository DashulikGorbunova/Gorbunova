namespace WebApplication1.Models;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expires { get; set; }
    public string Role { get; set; } = string.Empty;
    public int UserId { get; set; }
}


