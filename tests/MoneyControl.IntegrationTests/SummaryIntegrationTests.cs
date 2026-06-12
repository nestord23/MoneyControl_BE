using FluentAssertions;
using MoneyControl.Models;
using MoneyControl.Repositories;

namespace MoneyControl.IntegrationTests;

[Collection("Database")]
public class SummaryIntegrationTests
{
    private readonly DbFixture _fixture;

    public SummaryIntegrationTests(DbFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetExpenseSummary_ReturnsCorrectTotals()
    {
        await _fixture.CleanAllTablesAsync();

        using var context = _fixture.CreateContext();
        var categoryRepo = new CategoryRepository(context);
        var expenseRepo = new ExpenseRepository(context);

        var cat = await categoryRepo.CreateAsync(new Category { Name = "Test" });

        var now = new DateTime(2026, 6, 12, 10, 0, 0, DateTimeKind.Utc);
        var weekStart = now.Date.AddDays(-(int)now.DayOfWeek);
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var yearStart = new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        await expenseRepo.CreateAsync(new Expense { Amount = 100m, Date = now, Type = ExpenseType.Fixed, CategoryId = cat.Id });
        await expenseRepo.CreateAsync(new Expense { Amount = 50m, Date = now, Type = ExpenseType.Variable, CategoryId = cat.Id });
        await expenseRepo.CreateAsync(new Expense { Amount = 30m, Date = now.AddDays(-10), Type = ExpenseType.Fixed, CategoryId = cat.Id });

        var summary = await expenseRepo.GetAggregatedSummaryAsync(now.Date, weekStart, monthStart, yearStart);

        summary.Should().NotBeNull();
        summary.TotalByDay.Should().Be(150m);
        summary.TotalByWeek.Should().Be(150m);
        summary.TotalByMonth.Should().Be(150m);
        summary.TotalByYear.Should().Be(180m);
        summary.TotalFixed.Should().Be(130m);
        summary.TotalVariable.Should().Be(50m);
    }

    [Fact]
    public async Task GetLoanSummary_ReturnsCorrectTotals()
    {
        await _fixture.CleanAllTablesAsync();

        using var context = _fixture.CreateContext();
        var repo = new LoanRepository(context);

        await repo.CreateAsync(new Loan { Amount = 1000m, LenderName = "A", Date = DateTime.UtcNow, Status = LoanStatus.Pending });
        await repo.CreateAsync(new Loan { Amount = 500m, LenderName = "B", Date = DateTime.UtcNow, Status = LoanStatus.Pending });
        await repo.CreateAsync(new Loan { Amount = 2000m, LenderName = "C", Date = DateTime.UtcNow, Status = LoanStatus.Paid });

        var summary = await repo.GetSummaryAsync();

        summary.TotalPending.Should().Be(1500m);
        summary.TotalPaid.Should().Be(2000m);
        summary.PendingCount.Should().Be(2);
        summary.PaidCount.Should().Be(1);
    }
}
