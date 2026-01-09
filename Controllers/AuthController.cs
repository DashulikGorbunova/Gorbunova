using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IJwtService _jwtService;
    private readonly IUserService _userService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IJwtService jwtService, 
        IUserService userService,
        ILogger<AuthController> logger)
    {
        _jwtService = jwtService;
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Login with username and password
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Username and password are required" });
        }

        var user = await _userService.GetUserByUsernameAsync(request.Username);
        if (user == null)
        {
            _logger.LogWarning("Login attempt with non-existent username: {Username}", request.Username);
            return Unauthorized(new { message = "Invalid credentials" });
        }

        if (!user.IsActive)
        {
            _logger.LogWarning("Login attempt with inactive user: {Username}", request.Username);
            return Unauthorized(new { message = "User account is inactive" });
        }

        var isValidPassword = await _userService.ValidatePasswordAsync(request.Password, user.PasswordHash);
        if (!isValidPassword)
        {
            _logger.LogWarning("Login attempt with invalid password for user: {Username}", request.Username);
            return Unauthorized(new { message = "Invalid credentials" });
        }

        var token = _jwtService.GenerateToken(user.Username, user.Role, user.Id);
        var expires = DateTime.UtcNow.AddHours(1);

        _logger.LogInformation("Login successful for user: {Username} with role: {Role}", user.Username, user.Role);

        return Ok(new LoginResponse
        {
            Token = token,
            Expires = expires,
            Role = user.Role,
            UserId = user.Id
        });
    }
}
