using Testcontainers.PostgreSql;

namespace WebApplication1.Tests;

public class DatabaseFixture : IAsyncLifetime
{
    public PostgreSqlContainer PostgresContainer { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        PostgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("test_db")
            .WithUsername("test_user")
            .WithPassword("test_password")
            .Build();

        await PostgresContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        if (PostgresContainer != null)
        {
            await PostgresContainer.DisposeAsync();
        }
    }
}

