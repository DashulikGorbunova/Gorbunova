using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;
using Moq;
using Npgsql;
using Testcontainers.PostgreSql;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Repositories;
using Xunit;

namespace WebApplication1.Tests;

public class FlowerCategoryRepositoryTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
{
    private readonly FlowerCategoryRepository _repository;
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly Mock<ILogger<FlowerCategoryRepository>> _mockLogger;
    private readonly PostgreSqlContainer _postgresContainer;
    private readonly string _connectionString;

    public FlowerCategoryRepositoryTests(DatabaseFixture fixture)
    {
        _postgresContainer = fixture.PostgresContainer;
        _connectionString = _postgresContainer.GetConnectionString();
        
        _connectionFactory = new TestDbConnectionFactory(_connectionString);
        _mockLogger = new Mock<ILogger<FlowerCategoryRepository>>();
        _repository = new FlowerCategoryRepository(_connectionFactory, _mockLogger.Object);
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

        await connection.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS flower_categories (
                id SERIAL PRIMARY KEY,
                name VARCHAR(255) NOT NULL,
                description VARCHAR(1000),
                is_active BOOLEAN NOT NULL DEFAULT true,
                created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                updated_at TIMESTAMP
            )");
    }

    private async Task SeedTestDataAsync()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await connection.ExecuteAsync(@"
            INSERT INTO flower_categories (id, name, description, is_active, created_at)
            VALUES 
                (1, 'Roses', 'Beautiful roses', true, CURRENT_TIMESTAMP),
                (2, 'Tulips', 'Spring tulips', true, CURRENT_TIMESTAMP),
                (3, 'Inactive Category', 'Inactive', false, CURRENT_TIMESTAMP)
            ON CONFLICT (id) DO NOTHING");
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCategories()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        var categories = result.ToList();
        Assert.True(categories.Count >= 3);
        Assert.Contains(categories, c => c.Name == "Roses");
        Assert.Contains(categories, c => c.Name == "Tulips");
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCategory_WhenExists()
    {
        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Roses", result.Name);
        Assert.Equal("Beautiful roses", result.Description);
        Assert.True(result.IsActive);
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
    public async Task AddAsync_CreatesNewCategory()
    {
        // Arrange
        var newCategory = new FlowerCategory
        {
            Name = "New Category",
            Description = "New Description",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        await _repository.AddAsync(newCategory);
        await _repository.SaveChangesAsync();

        // Assert
        Assert.True(newCategory.Id > 0);
        
        var retrieved = await _repository.GetByIdAsync(newCategory.Id);
        Assert.NotNull(retrieved);
        Assert.Equal("New Category", retrieved.Name);
        Assert.Equal("New Description", retrieved.Description);
        Assert.True(retrieved.IsActive);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingCategory()
    {
        // Arrange
        var category = await _repository.GetByIdAsync(1);
        Assert.NotNull(category);
        
        var originalName = category.Name;
        category.Name = "Updated Roses";
        category.Description = "Updated Description";
        category.IsActive = false;
        category.UpdatedAt = DateTime.UtcNow;

        // Act
        await _repository.UpdateAsync(category);
        await _repository.SaveChangesAsync();

        // Assert
        var updated = await _repository.GetByIdAsync(1);
        Assert.NotNull(updated);
        Assert.Equal("Updated Roses", updated.Name);
        Assert.Equal("Updated Description", updated.Description);
        Assert.False(updated.IsActive);
        Assert.NotNull(updated.UpdatedAt);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsException_WhenCategoryNotFound()
    {
        // Arrange
        var category = new FlowerCategory
        {
            Id = 999,
            Name = "Non-existent",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(async () =>
        {
            await _repository.UpdateAsync(category);
            await _repository.SaveChangesAsync();
        });
    }

    [Fact]
    public async Task DeleteAsync_DeletesCategory()
    {
        // Arrange
        var newCategory = new FlowerCategory
        {
            Name = "Category To Delete",
            Description = "Will be deleted",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        
        await _repository.AddAsync(newCategory);
        await _repository.SaveChangesAsync();
        var categoryId = newCategory.Id;
        
        // Verify it exists
        var beforeDelete = await _repository.GetByIdAsync(categoryId);
        Assert.NotNull(beforeDelete);

        // Act
        await _repository.DeleteAsync(newCategory);
        await _repository.SaveChangesAsync();

        // Assert
        var afterDelete = await _repository.GetByIdAsync(categoryId);
        Assert.Null(afterDelete);
    }

    [Fact]
    public async Task SaveChangesAsync_CompletesSuccessfully()
    {
        // Act & Assert - Should not throw
        await _repository.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsCategoriesInOrder()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        var categories = result.ToList();
        Assert.True(categories.Count >= 2);
        
        // Should be ordered by name
        var names = categories.Select(c => c.Name).ToList();
        var sortedNames = names.OrderBy(n => n).ToList();
        Assert.Equal(sortedNames, names);
    }
}

