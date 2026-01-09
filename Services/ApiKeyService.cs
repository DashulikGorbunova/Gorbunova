using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models.Entities;

namespace WebApplication1.Services;

public class ApiKeyService : IApiKeyService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ApiKeyService> _logger;

    public ApiKeyService(AppDbContext context, ILogger<ApiKeyService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> ValidateApiKeyAsync(string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return false;
        }

        var key = await GetApiKeyAsync(apiKey);
        
        if (key == null)
        {
            return false;
        }

        if (!key.IsActive)
        {
            _logger.LogWarning("API Key is inactive: {ApiKey}", apiKey);
            return false;
        }

        if (key.ExpiresAt.HasValue && key.ExpiresAt.Value < DateTime.UtcNow)
        {
            _logger.LogWarning("API Key has expired: {ApiKey}", apiKey);
            return false;
        }

        return true;
    }

    public async Task<ApiKey?> GetApiKeyAsync(string key)
    {
        return await _context.ApiKeys
            .FirstOrDefaultAsync(k => k.Key == key);
    }
}

