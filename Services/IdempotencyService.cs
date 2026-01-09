using WebApplication1.Models.DTO;

namespace WebApplication1.Services;

public interface IIdempotencyService
{
    Task<string?> GetResponseAsync(string idempotencyKey);
    Task StoreResponseAsync(string idempotencyKey, string response, TimeSpan expiry);
}

public class IdempotencyService : IIdempotencyService
{
    private readonly IRedisCacheService _cache;
    private readonly ILogger<IdempotencyService> _logger;

    public IdempotencyService(IRedisCacheService cache, ILogger<IdempotencyService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<string?> GetResponseAsync(string idempotencyKey)
    {
        var cacheKey = $"idempotency:{idempotencyKey}";
        return await _cache.GetAsync<string>(cacheKey);
    }

    public async Task StoreResponseAsync(string idempotencyKey, string response, TimeSpan expiry)
    {
        var cacheKey = $"idempotency:{idempotencyKey}";
        await _cache.SetAsync(cacheKey, response, expiry);
        _logger.LogInformation("Stored idempotency response for key: {Key}", idempotencyKey);
    }
}

