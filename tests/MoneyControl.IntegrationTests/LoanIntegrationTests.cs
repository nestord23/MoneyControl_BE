using FluentAssertions;
using MoneyControl.Models;
using MoneyControl.Repositories;

namespace MoneyControl.IntegrationTests;

[Collection("Database")]
public class LoanIntegrationTests
{
    private readonly DbFixture _fixture;

    public LoanIntegrationTests(DbFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateLoan_Then_GetById_ReturnsSameData()
    {
        await _fixture.CleanAllTablesAsync();

        using var context = _fixture.CreateContext();
        var repo = new LoanRepository(context);

        var created = await repo.CreateAsync(new Loan
        {
            Amount = 10000m,
            LenderName = "Bank",
            Date = new DateTime(2026, 6, 10, 12, 0, 0, DateTimeKind.Utc),
            Status = LoanStatus.Pending
        });

        created.Id.Should().BeGreaterThan(0);

        var fetched = await repo.GetByIdAsync(created.Id);
        fetched.Should().NotBeNull();
        fetched!.Amount.Should().Be(10000m);
        fetched.LenderName.Should().Be("Bank");
        fetched.Status.Should().Be(LoanStatus.Pending);
    }

    [Fact]
    public async Task UpdateLoan_ModifiesData()
    {
        await _fixture.CleanAllTablesAsync();

        using var context = _fixture.CreateContext();
        var repo = new LoanRepository(context);

        var created = await repo.CreateAsync(new Loan
        {
            Amount = 5000m,
            LenderName = "Friend",
            Date = DateTime.UtcNow,
            Status = LoanStatus.Pending
        });

        var updated = await repo.UpdateAsync(new Loan
        {
            Id = created.Id,
            Amount = 6000m,
            LenderName = "Friend Updated",
            Date = created.Date,
            Status = LoanStatus.Paid
        });

        updated.Should().NotBeNull();
        updated!.Amount.Should().Be(6000m);
        updated.LenderName.Should().Be("Friend Updated");
        updated.Status.Should().Be(LoanStatus.Paid);
    }

    [Fact]
    public async Task DeleteLoan_RemovesAndReturnsTrue()
    {
        await _fixture.CleanAllTablesAsync();

        using var context = _fixture.CreateContext();
        var repo = new LoanRepository(context);

        var created = await repo.CreateAsync(new Loan
        {
            Amount = 2000m,
            LenderName = "Test",
            Date = DateTime.UtcNow
        });

        var deleted = await repo.DeleteAsync(created.Id);
        deleted.Should().BeTrue();

        var fetched = await repo.GetByIdAsync(created.Id);
        fetched.Should().BeNull();
    }
}
