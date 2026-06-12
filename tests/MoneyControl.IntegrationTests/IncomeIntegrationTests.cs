using FluentAssertions;
using MoneyControl.Models;
using MoneyControl.Repositories;

namespace MoneyControl.IntegrationTests;

[Collection("Database")]
public class IncomeIntegrationTests
{
    private readonly DbFixture _fixture;

    public IncomeIntegrationTests(DbFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateIncome_Then_GetById_ReturnsSameData()
    {
        await _fixture.CleanAllTablesAsync();

        using var context = _fixture.CreateContext();
        var repo = new IncomeRepository(context);

        var created = await repo.CreateAsync(new Income
        {
            Amount = 1500.00m,
            Date = new DateTime(2026, 6, 10, 12, 0, 0, DateTimeKind.Utc),
            Description = "Freelance payment"
        });

        created.Id.Should().BeGreaterThan(0);

        var fetched = await repo.GetByIdAsync(created.Id);
        fetched.Should().NotBeNull();
        fetched!.Amount.Should().Be(1500.00m);
        fetched.Description.Should().Be("Freelance payment");
    }

    [Fact]
    public async Task UpdateIncome_ModifiesData()
    {
        await _fixture.CleanAllTablesAsync();

        using var context = _fixture.CreateContext();
        var repo = new IncomeRepository(context);

        var created = await repo.CreateAsync(new Income
        {
            Amount = 1000m,
            Date = DateTime.UtcNow,
            Description = "Original"
        });

        var updated = await repo.UpdateAsync(new Income
        {
            Id = created.Id,
            Amount = 2000m,
            Date = created.Date,
            Description = "Updated"
        });

        updated.Should().NotBeNull();
        updated!.Amount.Should().Be(2000m);
        updated.Description.Should().Be("Updated");
    }

    [Fact]
    public async Task DeleteIncome_RemovesAndReturnsTrue()
    {
        await _fixture.CleanAllTablesAsync();

        using var context = _fixture.CreateContext();
        var repo = new IncomeRepository(context);

        var created = await repo.CreateAsync(new Income
        {
            Amount = 500m,
            Date = DateTime.UtcNow
        });

        var deleted = await repo.DeleteAsync(created.Id);
        deleted.Should().BeTrue();

        var fetched = await repo.GetByIdAsync(created.Id);
        fetched.Should().BeNull();
    }
}
