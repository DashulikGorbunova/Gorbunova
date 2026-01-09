using System.Security.Claims;
using WebApplication1.Services;

namespace WebApplication1.Middleware;

public class ApiKeyAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiKeyAuthenticationMiddleware> _logger;

    public ApiKeyAuthenticationMiddleware(RequestDelegate next, ILogger<ApiKeyAuthenticationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IApiKeyService apiKeyService)
    {
        if (context.Request.Headers.TryGetValue("X-API-KEY", out var apiKeyHeader))
        {
            var apiKey = apiKeyHeader.ToString();
            var isValid = await apiKeyService.ValidateApiKeyAsync(apiKey);

            if (isValid)
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, "ApiKey"),
                    new Claim(ClaimTypes.NameIdentifier, "ApiKey"),
                    new Claim("ApiKey", apiKey)
                };

                var identity = new ClaimsIdentity(claims, "ApiKey");
                context.User = new ClaimsPrincipal(identity);
                
                _logger.LogInformation("API Key authentication successful");
            }
            else
            {
                _logger.LogWarning("Invalid API Key provided");
            }
        }

        await _next(context);
    }
}

