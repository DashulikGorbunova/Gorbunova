using WebApplication1.Models;
using WebApplication1.Models.DTO;
using WebApplication1.Repositories;

namespace WebApplication1.Services;

public interface IFlowerService
{
    Task<IEnumerable<Flower>> GetAllAsync();
    Task<Flower?> GetByIdAsync(int id);
    Task<IEnumerable<Flower>> GetByCategoryIdAsync(int categoryId);
    Task<IEnumerable<Flower>> GetByColorAsync(string color);
    Task<PagedResponseDto<Flower>> GetFilteredAsync(FlowerFilterDto filter);
    Task<Flower> CreateAsync(FlowerCreateDto dto);
    Task<bool> UpdateAsync(int id, FlowerUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}

public class FlowerService : IFlowerService
{
    private const string AllFlowersCacheKey = "flowers:all";

    private readonly IFlowerRepository _flowerRepository;
    private readonly IRedisCacheService _cache;
    private readonly ILogger<FlowerService> _logger;

    public FlowerService(IFlowerRepository flowerRepository, IRedisCacheService cache, ILogger<FlowerService> logger)
    {
        _flowerRepository = flowerRepository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IEnumerable<Flower>> GetAllAsync()
    {
        try
        {
            var cached = await _cache.GetAsync<IEnumerable<Flower>>(AllFlowersCacheKey);
            if (cached != null)
            {
                return cached;
            }

            var flowers = await _flowerRepository.GetAllAsync();
            await _cache.SetAsync(AllFlowersCacheKey, flowers, TimeSpan.FromMinutes(5));
            return flowers;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all flowers");
            throw;
        }
    }

    public async Task<Flower?> GetByIdAsync(int id)
    {
        try
        {
            var cacheKey = $"flowers:{id}";
            var cached = await _cache.GetAsync<Flower>(cacheKey);
            if (cached != null)
            {
                return cached;
            }

            var flower = await _flowerRepository.GetByIdAsync(id);
            if (flower != null)
            {
                await _cache.SetAsync(cacheKey, flower, TimeSpan.FromMinutes(5));
            }

            return flower;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting flower by id: {FlowerId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Flower>> GetByCategoryIdAsync(int categoryId)
    {
        try
        {
            var cacheKey = $"flowers:category:{categoryId}";
            var cached = await _cache.GetAsync<IEnumerable<Flower>>(cacheKey);
            if (cached != null)
            {
                return cached;
            }

            var flowers = await _flowerRepository.GetByCategoryIdAsync(categoryId);
            await _cache.SetAsync(cacheKey, flowers, TimeSpan.FromMinutes(5));
            return flowers;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting flowers by category: {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<IEnumerable<Flower>> GetByColorAsync(string color)
    {
        try
        {
            var cacheKey = $"flowers:color:{color.ToLower()}";
            var cached = await _cache.GetAsync<IEnumerable<Flower>>(cacheKey);
            if (cached != null)
            {
                return cached;
            }

            var flowers = await _flowerRepository.GetByColorAsync(color);
            await _cache.SetAsync(cacheKey, flowers, TimeSpan.FromMinutes(5));
            return flowers;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting flowers by color: {Color}", color);
            throw;
        }
    }

    public async Task<PagedResponseDto<Flower>> GetFilteredAsync(FlowerFilterDto filter)
    {
        try
        {
            var cacheKey = $"flowers:filter:{filter.Page}:{filter.PageSize}:{filter.Search}:{filter.CategoryId}:{filter.Color}:{filter.IsAvailable}:{filter.MinPrice}:{filter.MaxPrice}";
            var cached = await _cache.GetAsync<PagedResponseDto<Flower>>(cacheKey);
            if (cached != null)
            {
                return cached;
            }

            var (items, total) = await _flowerRepository.GetFilteredAsync(filter);
            var result = new PagedResponseDto<Flower>
            {
                Items = items,
                Total = total,
                Page = filter.Page,
                PageSize = filter.PageSize
            };

            await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(2));
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting filtered flowers");
            throw;
        }
    }

    public async Task<Flower> CreateAsync(FlowerCreateDto dto)
    {
        try
        {
            var flower = new Flower
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Quantity = dto.Quantity,
                Color = dto.Color,
                Season = dto.Season,
                ImageUrl = dto.ImageUrl,
                CategoryId = dto.CategoryId,
                IsAvailable = dto.IsAvailable,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };
            
            await _flowerRepository.AddAsync(flower);
            await _flowerRepository.SaveChangesAsync();

            var cacheKey = $"flowers:{flower.Id}";
            await _cache.SetAsync(cacheKey, flower, TimeSpan.FromMinutes(5));
            await _cache.DeleteAsync(AllFlowersCacheKey); // invalidate list
            if (flower.CategoryId.HasValue)
            {
                await _cache.DeleteAsync($"flowers:category:{flower.CategoryId.Value}");
            }
            if (!string.IsNullOrEmpty(flower.Color))
            {
                await _cache.DeleteAsync($"flowers:color:{flower.Color.ToLower()}");
            }

            _logger.LogInformation("Flower created successfully: {FlowerId}, Name: {FlowerName}", flower.Id, flower.Name);
            return flower;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating flower: {FlowerName}", dto.Name);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(int id, FlowerUpdateDto dto)
    {
        try
        {
            var existingFlower = await _flowerRepository.GetByIdAsync(id);
            if (existingFlower == null)
            {
                _logger.LogWarning("Flower not found for update: {FlowerId}", id);
                return false;
            }

            existingFlower.Name = dto.Name;
            existingFlower.Description = dto.Description;
            existingFlower.Price = dto.Price;
            existingFlower.Quantity = dto.Quantity;
            existingFlower.Color = dto.Color;
            existingFlower.Season = dto.Season;
            existingFlower.ImageUrl = dto.ImageUrl;
            existingFlower.CategoryId = dto.CategoryId;
            existingFlower.IsAvailable = dto.IsAvailable;
            existingFlower.UpdatedAt = DateTime.UtcNow;

            await _flowerRepository.UpdateAsync(existingFlower);
            await _flowerRepository.SaveChangesAsync();

            var cacheKey = $"flowers:{id}";
            await _cache.SetAsync(cacheKey, existingFlower, TimeSpan.FromMinutes(5));
            await _cache.DeleteAsync(AllFlowersCacheKey);
            if (existingFlower.CategoryId.HasValue)
            {
                await _cache.DeleteAsync($"flowers:category:{existingFlower.CategoryId.Value}");
            }
            if (!string.IsNullOrEmpty(existingFlower.Color))
            {
                await _cache.DeleteAsync($"flowers:color:{existingFlower.Color.ToLower()}");
            }

            _logger.LogInformation("Flower updated successfully: {FlowerId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating flower: {FlowerId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var flower = await _flowerRepository.GetByIdAsync(id);
            if (flower == null)
            {
                _logger.LogWarning("Flower not found for deletion: {FlowerId}", id);
                return false;
            }

            await _flowerRepository.DeleteAsync(flower);
            await _flowerRepository.SaveChangesAsync();

            var cacheKey = $"flowers:{id}";
            await _cache.DeleteAsync(cacheKey);
            await _cache.DeleteAsync(AllFlowersCacheKey);
            if (flower.CategoryId.HasValue)
            {
                await _cache.DeleteAsync($"flowers:category:{flower.CategoryId.Value}");
            }
            if (!string.IsNullOrEmpty(flower.Color))
            {
                await _cache.DeleteAsync($"flowers:color:{flower.Color.ToLower()}");
            }

            _logger.LogInformation("Flower deleted successfully: {FlowerId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting flower: {FlowerId}", id);
            throw;
        }
    }
}

