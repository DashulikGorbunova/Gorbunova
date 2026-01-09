namespace WebApplication1.Services;

public interface IApiKeyService
{
    Task<bool> ValidateApiKeyAsync(string apiKey);
    Task<Models.Entities.ApiKey?> GetApiKeyAsync(string key);
}

