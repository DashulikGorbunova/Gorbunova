using Dapper;
using System.Data;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.DTO;

namespace WebApplication1.Repositories;

public class FlowerRepository : IFlowerRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<FlowerRepository> _logger;

    public FlowerRepository(IDbConnectionFactory connectionFactory, ILogger<FlowerRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<IEnumerable<Flower>> GetAllAsync()
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT f.id AS Id, f.name AS Name, f.description AS Description, 
                       f.price AS Price, f.quantity AS Quantity, 
                       f.color AS Color, f.season AS Season, f.image_url AS ImageUrl,
                       f.category_id AS CategoryId, f.is_available AS IsAvailable,
                       f.created_at AS CreatedAt, f.updated_at AS UpdatedAt,
                       c.id AS Category_Id, c.name AS Category_Name, c.description AS Category_Description,
                       c.is_active AS Category_IsActive, c.created_at AS Category_CreatedAt, c.updated_at AS Category_UpdatedAt
                FROM flowers f
                LEFT JOIN flower_categories c ON f.category_id = c.id
                ORDER BY f.created_at DESC";
            
            return await connection.QueryAsync<Flower, FlowerCategory, Flower>(sql, (flower, category) =>
            {
                flower.Category = category;
                return flower;
            }, splitOn: "Category_Id");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all flowers from database");
            throw;
        }
    }

    public async Task<Flower?> GetByIdAsync(int id)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT f.id AS Id, f.name AS Name, f.description AS Description, 
                       f.price AS Price, f.quantity AS Quantity, 
                       f.color AS Color, f.season AS Season, f.image_url AS ImageUrl,
                       f.category_id AS CategoryId, f.is_available AS IsAvailable,
                       f.created_at AS CreatedAt, f.updated_at AS UpdatedAt,
                       c.id AS Category_Id, c.name AS Category_Name, c.description AS Category_Description,
                       c.is_active AS Category_IsActive, c.created_at AS Category_CreatedAt, c.updated_at AS Category_UpdatedAt
                FROM flowers f
                LEFT JOIN flower_categories c ON f.category_id = c.id
                WHERE f.id = @Id";
            
            var result = await connection.QueryAsync<Flower, FlowerCategory, Flower>(sql, (flower, category) =>
            {
                flower.Category = category;
                return flower;
            }, new { Id = id }, splitOn: "Category_Id");
            
            return result.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting flower by id from database: {FlowerId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Flower>> GetByCategoryIdAsync(int categoryId)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT f.id AS Id, f.name AS Name, f.description AS Description, 
                       f.price AS Price, f.quantity AS Quantity, 
                       f.color AS Color, f.season AS Season, f.image_url AS ImageUrl,
                       f.category_id AS CategoryId, f.is_available AS IsAvailable,
                       f.created_at AS CreatedAt, f.updated_at AS UpdatedAt
                FROM flowers f
                WHERE f.category_id = @CategoryId AND f.is_available = true
                ORDER BY f.name";
            
            return await connection.QueryAsync<Flower>(sql, new { CategoryId = categoryId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting flowers by category from database: {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<IEnumerable<Flower>> GetByColorAsync(string color)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT f.id AS Id, f.name AS Name, f.description AS Description, 
                       f.price AS Price, f.quantity AS Quantity, 
                       f.color AS Color, f.season AS Season, f.image_url AS ImageUrl,
                       f.category_id AS CategoryId, f.is_available AS IsAvailable,
                       f.created_at AS CreatedAt, f.updated_at AS UpdatedAt
                FROM flowers f
                WHERE LOWER(f.color) = LOWER(@Color) AND f.is_available = true
                ORDER BY f.name";
            
            return await connection.QueryAsync<Flower>(sql, new { Color = color });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting flowers by color from database: {Color}", color);
            throw;
        }
    }

    public async Task AddAsync(Flower flower)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            using var transaction = connection.BeginTransaction();
            
            try
            {
            const string sql = @"
                INSERT INTO flowers (name, description, price, quantity, color, season, image_url, category_id, is_available, created_at, updated_at)
                VALUES (@Name, @Description, @Price, @Quantity, @Color, @Season, @ImageUrl, @CategoryId, @IsAvailable, @CreatedAt, @UpdatedAt)
                RETURNING id";
            
                var id = await connection.QuerySingleAsync<int>(sql, new
                {
                    flower.Name,
                    flower.Description,
                    flower.Price,
                    flower.Quantity,
                    flower.Color,
                    flower.Season,
                    flower.ImageUrl,
                    flower.CategoryId,
                    flower.IsAvailable,
                    flower.CreatedAt,
                    flower.UpdatedAt
                }, transaction);
                
                flower.Id = id;
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding flower to database: {FlowerName}", flower.Name);
            throw;
        }
    }

    public async Task UpdateAsync(Flower flower)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            using var transaction = connection.BeginTransaction();
            
            try
            {
            const string sql = @"
                UPDATE flowers 
                SET name = @Name, 
                    description = @Description, 
                    price = @Price, 
                    quantity = @Quantity,
                    color = @Color,
                    season = @Season,
                    image_url = @ImageUrl,
                    category_id = @CategoryId,
                    is_available = @IsAvailable,
                    updated_at = @UpdatedAt
                WHERE id = @Id";
            
                await connection.ExecuteAsync(sql, new
                {
                    flower.Id,
                    flower.Name,
                    flower.Description,
                    flower.Price,
                    flower.Quantity,
                    flower.Color,
                    flower.Season,
                    flower.ImageUrl,
                    flower.CategoryId,
                    flower.IsAvailable,
                    flower.UpdatedAt
                }, transaction);
                
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating flower in database: {FlowerId}", flower.Id);
            throw;
        }
    }

    public async Task DeleteAsync(Flower flower)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            using var transaction = connection.BeginTransaction();
            
            try
            {
                const string sql = "DELETE FROM flowers WHERE id = @Id";
                
                await connection.ExecuteAsync(sql, new { flower.Id }, transaction);
                
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting flower from database: {FlowerId}", flower.Id);
            throw;
        }
    }

    public async Task<(IEnumerable<Flower> Items, int Total)> GetFilteredAsync(FlowerFilterDto filter)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            using var transaction = connection.BeginTransaction();
            
            try
            {
                // Build WHERE clause
                var whereConditions = new List<string>();
                var parameters = new DynamicParameters();

                if (!string.IsNullOrWhiteSpace(filter.Search))
                {
                    whereConditions.Add("(LOWER(f.name) LIKE @Search OR LOWER(f.description) LIKE @Search)");
                    parameters.Add("Search", $"%{filter.Search.ToLower()}%");
                }

                if (filter.CategoryId.HasValue)
                {
                    whereConditions.Add("f.category_id = @CategoryId");
                    parameters.Add("CategoryId", filter.CategoryId.Value);
                }

                if (!string.IsNullOrWhiteSpace(filter.Color))
                {
                    whereConditions.Add("LOWER(f.color) = LOWER(@Color)");
                    parameters.Add("Color", filter.Color);
                }

                if (filter.IsAvailable.HasValue)
                {
                    whereConditions.Add("f.is_available = @IsAvailable");
                    parameters.Add("IsAvailable", filter.IsAvailable.Value);
                }

                if (filter.MinPrice.HasValue)
                {
                    whereConditions.Add("f.price >= @MinPrice");
                    parameters.Add("MinPrice", filter.MinPrice.Value);
                }

                if (filter.MaxPrice.HasValue)
                {
                    whereConditions.Add("f.price <= @MaxPrice");
                    parameters.Add("MaxPrice", filter.MaxPrice.Value);
                }

                var whereClause = whereConditions.Any() 
                    ? "WHERE " + string.Join(" AND ", whereConditions)
                    : "";

                // Get total count
                var countSql = $@"
                    SELECT COUNT(*) 
                    FROM flowers f
                    {whereClause}";

                var total = await connection.QuerySingleAsync<int>(countSql, parameters, transaction);

                // Get paginated results
                var offset = (filter.Page - 1) * filter.PageSize;
                parameters.Add("Offset", offset);
                parameters.Add("PageSize", filter.PageSize);

                var sql = $@"
                    SELECT f.id AS Id, f.name AS Name, f.description AS Description, 
                           f.price AS Price, f.quantity AS Quantity, 
                           f.color AS Color, f.season AS Season, f.image_url AS ImageUrl,
                           f.category_id AS CategoryId, f.is_available AS IsAvailable,
                           f.created_at AS CreatedAt, f.updated_at AS UpdatedAt,
                           c.id AS Category_Id, c.name AS Category_Name, c.description AS Category_Description,
                           c.is_active AS Category_IsActive, c.created_at AS Category_CreatedAt, c.updated_at AS Category_UpdatedAt
                    FROM flowers f
                    LEFT JOIN flower_categories c ON f.category_id = c.id
                    {whereClause}
                    ORDER BY f.created_at DESC
                    LIMIT @PageSize OFFSET @Offset";

                var items = await connection.QueryAsync<Flower, FlowerCategory, Flower>(
                    sql, 
                    (flower, category) =>
                    {
                        flower.Category = category;
                        return flower;
                    }, 
                    parameters, 
                    transaction,
                    splitOn: "Category_Id");

                transaction.Commit();
                return (items, total);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting filtered flowers from database");
            throw;
        }
    }

    public Task SaveChangesAsync()
    {
        // Dapper выполняет операции напрямую, SaveChanges не нужен
        return Task.CompletedTask;
    }
}

