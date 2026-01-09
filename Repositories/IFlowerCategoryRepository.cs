using WebApplication1.Models;

namespace WebApplication1.Repositories;

public interface IFlowerCategoryRepository
{
    Task<IEnumerable<FlowerCategory>> GetAllAsync();
    Task<FlowerCategory?> GetByIdAsync(int id);
    Task AddAsync(FlowerCategory category);
    Task UpdateAsync(FlowerCategory category);
    Task DeleteAsync(FlowerCategory category);
    Task SaveChangesAsync();
}

