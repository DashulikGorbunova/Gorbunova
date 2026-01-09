using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Services;

public interface IFlowerCategoryService
{
    Task<IEnumerable<FlowerCategory>> GetAllAsync();
    Task<FlowerCategory?> GetByIdAsync(int id);
    Task<FlowerCategory> CreateAsync(FlowerCategory category);
    Task<bool> UpdateAsync(int id, FlowerCategory category);
    Task<bool> DeleteAsync(int id);
}

public class FlowerCategoryService : IFlowerCategoryService
{
    private const string AllCategoriesCacheKey = "flower_categories:all";

    private readonly IFlowerCategoryRepository _categoryRepository;
    private readonly IRedisCacheService _cache;
    private readonly ILogger<FlowerCategoryService> _logger;

    public FlowerCategoryService(IFlowerCategoryRepository categoryRepository, IRedisCacheService cache, ILogger<FlowerCategoryService> logger)
    {
        _categoryRepository = categoryRepository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IEnumerable<FlowerCategory>> GetAllAsync()
    {
        try
        {
            var cached = await _cache.GetAsync<IEnumerable<FlowerCategory>>(AllCategoriesCacheKey);
            if (cached != null)
            {
                return cached;
            }

            var categories = await _categoryRepository.GetAllAsync();
            await _cache.SetAsync(AllCategoriesCacheKey, categories, TimeSpan.FromMinutes(5));
            return categories;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all flower categories");
            throw;
        }
    }

    public async Task<FlowerCategory?> GetByIdAsync(int id)
    {
        try
        {
            var cacheKey = $"flower_categories:{id}";
            var cached = await _cache.GetAsync<FlowerCategory>(cacheKey);
            if (cached != null)
            {
                return cached;
            }

            var category = await _categoryRepository.GetByIdAsync(id);
            if (category != null)
            {
                await _cache.SetAsync(cacheKey, category, TimeSpan.FromMinutes(5));
            }

            return category;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting flower category by id: {CategoryId}", id);
            throw;
        }
    }

    public async Task<FlowerCategory> CreateAsync(FlowerCategory category)
    {
        try
        {
            category.CreatedAt = DateTime.UtcNow;
            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();

            var cacheKey = $"flower_categories:{category.Id}";
            await _cache.SetAsync(cacheKey, category, TimeSpan.FromMinutes(5));
            await _cache.DeleteAsync(AllCategoriesCacheKey); // invalidate list

            _logger.LogInformation("Flower category created successfully: {CategoryId}, Name: {CategoryName}", category.Id, category.Name);
            return category;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating flower category: {CategoryName}", category.Name);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(int id, FlowerCategory category)
    {
        try
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(id);
            if (existingCategory == null)
            {
                _logger.LogWarning("Flower category not found for update: {CategoryId}", id);
                return false;
            }

            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;
            existingCategory.IsActive = category.IsActive;
            existingCategory.UpdatedAt = DateTime.UtcNow;

            await _categoryRepository.UpdateAsync(existingCategory);
            await _categoryRepository.SaveChangesAsync();

            var cacheKey = $"flower_categories:{id}";
            await _cache.SetAsync(cacheKey, existingCategory, TimeSpan.FromMinutes(5));
            await _cache.DeleteAsync(AllCategoriesCacheKey);

            _logger.LogInformation("Flower category updated successfully: {CategoryId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating flower category: {CategoryId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Flower category not found for deletion: {CategoryId}", id);
                return false;
            }

            await _categoryRepository.DeleteAsync(category);
            await _categoryRepository.SaveChangesAsync();

            var cacheKey = $"flower_categories:{id}";
            await _cache.DeleteAsync(cacheKey);
            await _cache.DeleteAsync(AllCategoriesCacheKey);

            _logger.LogInformation("Flower category deleted successfully: {CategoryId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting flower category: {CategoryId}", id);
            throw;
        }
    }
}

