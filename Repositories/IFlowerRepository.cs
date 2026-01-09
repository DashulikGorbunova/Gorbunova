using WebApplication1.Models;
using WebApplication1.Models.DTO;

namespace WebApplication1.Repositories;

public interface IFlowerRepository
{
    Task<IEnumerable<Flower>> GetAllAsync();
    Task<Flower?> GetByIdAsync(int id);
    Task<IEnumerable<Flower>> GetByCategoryIdAsync(int categoryId);
    Task<IEnumerable<Flower>> GetByColorAsync(string color);
    Task<(IEnumerable<Flower> Items, int Total)> GetFilteredAsync(FlowerFilterDto filter);
    Task AddAsync(Flower flower);
    Task UpdateAsync(Flower flower);
    Task DeleteAsync(Flower flower);
    Task SaveChangesAsync();
}

