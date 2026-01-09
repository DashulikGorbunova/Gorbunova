using Dapper;
using System.Data;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Repositories;

public class FlowerCategoryRepository : IFlowerCategoryRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<FlowerCategoryRepository> _logger;

    public FlowerCategoryRepository(IDbConnectionFactory connectionFactory, ILogger<FlowerCategoryRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<IEnumerable<FlowerCategory>> GetAllAsync()
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT id AS Id, name AS Name, description AS Description, 
                       is_active AS IsActive, created_at AS CreatedAt, updated_at AS UpdatedAt 
                FROM flower_categories 
                ORDER BY name";
            
            return await connection.QueryAsync<FlowerCategory>(sql);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all flower categories from database");
            throw;
        }
    }

    public async Task<FlowerCategory?> GetByIdAsync(int id)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT id AS Id, name AS Name, description AS Description, 
                       is_active AS IsActive, created_at AS CreatedAt, updated_at AS UpdatedAt 
                FROM flower_categories 
                WHERE id = @Id";
            
            return await connection.QueryFirstOrDefaultAsync<FlowerCategory>(sql, new { Id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting flower category by id from database: {CategoryId}", id);
            throw;
        }
    }

    public async Task AddAsync(FlowerCategory category)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                INSERT INTO flower_categories (name, description, is_active, created_at, updated_at)
                VALUES (@Name, @Description, @IsActive, @CreatedAt, @UpdatedAt)
                RETURNING id";
            
            var id = await connection.QuerySingleAsync<int>(sql, new
            {
                category.Name,
                category.Description,
                category.IsActive,
                category.CreatedAt,
                category.UpdatedAt
            });
            
            category.Id = id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding flower category to database: {CategoryName}", category.Name);
            throw;
        }
    }

    public async Task UpdateAsync(FlowerCategory category)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                UPDATE flower_categories 
                SET name = @Name, 
                    description = @Description, 
                    is_active = @IsActive, 
                    updated_at = @UpdatedAt
                WHERE id = @Id";
            
            await connection.ExecuteAsync(sql, new
            {
                category.Id,
                category.Name,
                category.Description,
                category.IsActive,
                category.UpdatedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating flower category in database: {CategoryId}", category.Id);
            throw;
        }
    }

    public async Task DeleteAsync(FlowerCategory category)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "DELETE FROM flower_categories WHERE id = @Id";
            
            await connection.ExecuteAsync(sql, new { category.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting flower category from database: {CategoryId}", category.Id);
            throw;
        }
    }

    public Task SaveChangesAsync()
    {
        // Dapper выполняет операции напрямую, SaveChanges не нужен
        return Task.CompletedTask;
    }
}

