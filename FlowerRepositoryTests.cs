using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;
using Moq;
using Npgsql;
using Testcontainers.PostgreSql;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.DTO;
using WebApplication1.Repositories;
using Xunit;

namespace WebApplication1.Tests;

public class FlowerRepositoryTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
{
    private readonly FlowerRepository _repository;
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly Mock<ILogger<FlowerRepository>> _mockLogger;
    private readonly PostgreSqlContainer _postgresContainer;
    private readonly string _connectionString;

    public FlowerRepositoryTests(DatabaseFixture fixture)
    {
        _postgresContainer = fixture.PostgresContainer;
        _connectionString = _postgresContainer.GetConnectionString();
        
        _connectionFactory = new TestDbConnectionFactory(_connectionString);
        _mockLogger = new Mock<ILogger<FlowerRepository>>();
        _repository = new FlowerRepository(_connectionFactory, _mockLogger.Object);
    }

    public async Task InitializeAsync()
    {
        await CreateTestTablesAsync();
        await SeedTestDataAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    private async Task CreateTestTablesAsync()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        // Create flower_categories table
        await connection.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS flower_categories (
                id SERIAL PRIMARY KEY,
                name VARCHAR(255) NOT NULL,
                description VARCHAR(1000),
                is_active BOOLEAN NOT NULL DEFAULT true,
                created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                updated_at TIMESTAMP
            )");

        // Create flowers table
        await connection.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS flowers (
                id SERIAL PRIMARY KEY,
                name VARCHAR(255) NOT NULL,
                description VARCHAR(1000),
                price DECIMAL(18,2) NOT NULL,
                quantity INTEGER NOT NULL DEFAULT 0,
                color VARCHAR(50),
                season VARCHAR(50),
                image_url VARCHAR(500),
                category_id INTEGER,
                is_available BOOLEAN NOT NULL DEFAULT true,
                created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                updated_at TIMESTAMP,
                CONSTRAINT fk_flowers_category FOREIGN KEY (category_id) REFERENCES flower_categories(id) ON DELETE SET NULL
            )");
    }

    private async Task SeedTestDataAsync()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        // Insert test category
        await connection.ExecuteAsync(@"
            INSERT INTO flower_categories (id, name, description, is_active, created_at)
            VALUES (1, 'Test Category', 'Test Description', true, CURRENT_TIMESTAMP)
            ON CONFLICT (id) DO NOTHING");

        // Insert test flowers
        await connection.ExecuteAsync(@"
            INSERT INTO flowers (id, name, description, price, quantity, color, season, category_id, is_available, created_at)
            VALUES 
                (1, 'Test Rose', 'Red rose', 15.99, 10, 'Red', 'All', 1, true, CURRENT_TIMESTAMP),
                (2, 'Test Tulip', 'White tulip', 8.99, 5, 'White', 'Spring', 1, true, CURRENT_TIMESTAMP),
                (3, 'Test Lily', 'Pink lily', 12.99, 8, 'Pink', 'Summer', NULL, false, CURRENT_TIMESTAMP)
            ON CONFLICT (id) DO NOTHING");
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllFlowers()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        var flowers = result.ToList();
        Assert.True(flowers.Count >= 3);
        Assert.Contains(flowers, f => f.Name == "Test Rose");
        Assert.Contains(flowers, f => f.Name == "Test Tulip");
        Assert.Contains(flowers, f => f.Name == "Test Lily");
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsFlower_WhenExists()
    {
        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Rose", result.Name);
        Assert.Equal(15.99m, result.Price);
        Assert.Equal(10, result.Quantity);
        Assert.Equal("Red", result.Color);
        Assert.True(result.IsAvailable);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByCategoryIdAsync_ReturnsFlowers_ForCategory()
    {
        // Act
        var result = await _repository.GetByCategoryIdAsync(1);

        // Assert
        Assert.NotNull(result);
        var flowers = result.ToList();
        Assert.True(flowers.Count >= 2);
        Assert.All(flowers, f => Assert.True(f.IsAvailable));
        Assert.Contains(flowers, f => f.Name == "Test Rose");
        Assert.Contains(flowers, f => f.Name == "Test Tulip");
    }

    [Fact]
    public async Task GetByCategoryIdAsync_ReturnsEmpty_WhenCategoryNotFound()
    {
        // Act
        var result = await _repository.GetByCategoryIdAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByColorAsync_ReturnsFlowers_ForColor()
    {
        // Act
        var result = await _repository.GetByColorAsync("Red");

        // Assert
        Assert.NotNull(result);
        var flowers = result.ToList();
        Assert.True(flowers.Count >= 1);
        Assert.All(flowers, f => 
        {
            Assert.Equal("Red", f.Color, ignoreCase: true);
            Assert.True(f.IsAvailable);
        });
        Assert.Contains(flowers, f => f.Name == "Test Rose");
    }

    [Fact]
    public async Task GetByColorAsync_ReturnsEmpty_WhenColorNotFound()
    {
        // Act
        var result = await _repository.GetByColorAsync("Purple");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetFilteredAsync_ReturnsPaginatedResults()
    {
        // Arrange
        var filter = new FlowerFilterDto
        {
            Page = 1,
            PageSize = 2
        };

        // Act
        var (items, total) = await _repository.GetFilteredAsync(filter);

        // Assert
        Assert.NotNull(items);
        var flowers = items.ToList();
        Assert.True(flowers.Count <= 2);
        Assert.True(total >= 3);
    }

    [Fact]
    public async Task GetFilteredAsync_FiltersBySearch()
    {
        // Arrange
        var filter = new FlowerFilterDto
        {
            Page = 1,
            PageSize = 10,
            Search = "Rose"
        };

        // Act
        var (items, total) = await _repository.GetFilteredAsync(filter);

        // Assert
        Assert.NotNull(items);
        var flowers = items.ToList();
        Assert.True(flowers.Count >= 1);
        Assert.All(flowers, f => 
            Assert.Contains("Rose", f.Name, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task GetFilteredAsync_FiltersByCategoryId()
    {
        // Arrange
        var filter = new FlowerFilterDto
        {
            Page = 1,
            PageSize = 10,
            CategoryId = 1
        };

        // Act
        var (items, total) = await _repository.GetFilteredAsync(filter);

        // Assert
        Assert.NotNull(items);
        var flowers = items.ToList();
        Assert.True(flowers.Count >= 2);
        Assert.All(flowers, f => Assert.Equal(1, f.CategoryId));
    }

    [Fact]
    public async Task GetFilteredAsync_FiltersByIsAvailable()
    {
        // Arrange
        var filter = new FlowerFilterDto
        {
            Page = 1,
            PageSize = 10,
            IsAvailable = true
        };

        // Act
        var (items, total) = await _repository.GetFilteredAsync(filter);

        // Assert
        Assert.NotNull(items);
        var flowers = items.ToList();
        Assert.True(flowers.Count >= 2);
        Assert.All(flowers, f => Assert.True(f.IsAvailable));
    }

    [Fact]
    public async Task GetFilteredAsync_FiltersByPriceRange()
    {
        // Arrange
        var filter = new FlowerFilterDto
        {
            Page = 1,
            PageSize = 10,
            MinPrice = 10.00m,
            MaxPrice = 20.00m
        };

        // Act
        var (items, total) = await _repository.GetFilteredAsync(filter);

        // Assert
        Assert.NotNull(items);
        var flowers = items.ToList();
        Assert.True(flowers.Count >= 1);
        Assert.All(flowers, f => 
        {
            Assert.True(f.Price >= 10.00m);
            Assert.True(f.Price <= 20.00m);
        });
    }

    [Fact]
    public async Task AddAsync_CreatesNewFlower()
    {
        // Arrange
        var newFlower = new Flower
        {
            Name = "New Test Flower",
            Description = "New Description",
            Price = 20.99m,
            Quantity = 15,
            Color = "Blue",
            Season = "Winter",
            CategoryId = 1,
            IsAvailable = true,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        await _repository.AddAsync(newFlower);
        await _repository.SaveChangesAsync();

        // Assert
        Assert.True(newFlower.Id > 0);
        
        var retrieved = await _repository.GetByIdAsync(newFlower.Id);
        Assert.NotNull(retrieved);
        Assert.Equal("New Test Flower", retrieved.Name);
        Assert.Equal(20.99m, retrieved.Price);
        Assert.Equal(15, retrieved.Quantity);
        Assert.Equal("Blue", retrieved.Color);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingFlower()
    {
        // Arrange
        var flower = await _repository.GetByIdAsync(1);
        Assert.NotNull(flower);
        
        var originalName = flower.Name;
        flower.Name = "Updated Rose";
        flower.Price = 19.99m;
        flower.Quantity = 20;
        flower.UpdatedAt = DateTime.UtcNow;

        // Act
        await _repository.UpdateAsync(flower);
        await _repository.SaveChangesAsync();

        // Assert
        var updated = await _repository.GetByIdAsync(1);
        Assert.NotNull(updated);
        Assert.Equal("Updated Rose", updated.Name);
        Assert.Equal(19.99m, updated.Price);
        Assert.Equal(20, updated.Quantity);
        Assert.NotNull(updated.UpdatedAt);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsException_WhenFlowerNotFound()
    {
        // Arrange
        var flower = new Flower
        {
            Id = 999,
            Name = "Non-existent",
            Price = 10.00m,
            Quantity = 1,
            CreatedAt = DateTime.UtcNow
        };

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(async () =>
        {
            await _repository.UpdateAsync(flower);
            await _repository.SaveChangesAsync();
        });
    }

    [Fact]
    public async Task DeleteAsync_DeletesFlower()
    {
        // Arrange
        var newFlower = new Flower
        {
            Name = "Flower To Delete",
            Description = "Will be deleted",
            Price = 5.99m,
            Quantity = 1,
            CreatedAt = DateTime.UtcNow
        };
        
        await _repository.AddAsync(newFlower);
        await _repository.SaveChangesAsync();
        var flowerId = newFlower.Id;
        
        // Verify it exists
        var beforeDelete = await _repository.GetByIdAsync(flowerId);
        Assert.NotNull(beforeDelete);

        // Act
        await _repository.DeleteAsync(newFlower);
        await _repository.SaveChangesAsync();

        // Assert
        var afterDelete = await _repository.GetByIdAsync(flowerId);
        Assert.Null(afterDelete);
    }

    [Fact]
    public async Task SaveChangesAsync_CompletesSuccessfully()
    {
        // Act & Assert - Should not throw
        await _repository.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAllAsync_IncludesCategory_WhenCategoryExists()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        var flowers = result.ToList();
        var flowerWithCategory = flowers.FirstOrDefault(f => f.CategoryId == 1);
        Assert.NotNull(flowerWithCategory);
        Assert.NotNull(flowerWithCategory.Category);
        Assert.Equal("Test Category", flowerWithCategory.Category.Name);
    }

    [Fact]
    public async Task GetByIdAsync_IncludesCategory_WhenCategoryExists()
    {
        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Category);
        Assert.Equal("Test Category", result.Category.Name);
    }
}

