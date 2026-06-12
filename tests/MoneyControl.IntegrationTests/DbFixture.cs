using Microsoft.EntityFrameworkCore;
using MoneyControl.Data;
using MoneyControl.Models;
using Npgsql;
using Testcontainers.PostgreSql;

namespace MoneyControl.IntegrationTests;

public class DbFixture : IAsyncLifetime
{
    private PostgreSqlContainer _container = null!;
    private NpgsqlDataSource _dataSource = null!;

    public async Task InitializeAsync()
    {
        _container = new PostgreSqlBuilder("postgres:16-alpine")
            .Build();
        await _container.StartAsync();

        var builder = new NpgsqlDataSourceBuilder(_container.GetConnectionString());
        builder.MapEnum<ExpenseType>();
        builder.MapEnum<LoanStatus>();
        _dataSource = builder.Build();

        using var context = CreateContext();
        await context.Database.EnsureCreatedAsync();
    }

    public AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_dataSource)
            .Options;
        return new AppDbContext(options);
    }

    public async Task CleanAllTablesAsync()
    {
        using var context = CreateContext();
        await context.Database.ExecuteSqlRawAsync("DELETE FROM \"Expenses\"");
        await context.Database.ExecuteSqlRawAsync("DELETE FROM \"Incomes\"");
        await context.Database.ExecuteSqlRawAsync("DELETE FROM \"Loans\"");
        await context.Database.ExecuteSqlRawAsync("DELETE FROM \"Categories\"");
    }

    public async Task DisposeAsync()
    {
        await _dataSource.DisposeAsync();
        await _container.DisposeAsync();
    }
}
