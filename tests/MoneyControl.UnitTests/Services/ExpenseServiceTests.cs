using FluentAssertions;
using Moq;
using MoneyControl.DTOs;
using MoneyControl.Helpers;
using MoneyControl.Models;
using MoneyControl.Repositories;
using MoneyControl.Services;

namespace MoneyControl.UnitTests.Services;

public class ExpenseServiceTests
{
    private readonly Mock<IExpenseRepository> _expenseRepo;
    private readonly Mock<ICategoryRepository> _categoryRepo;
    private readonly ExpenseService _service;

    public ExpenseServiceTests()
    {
        _expenseRepo = new Mock<IExpenseRepository>();
        _categoryRepo = new Mock<ICategoryRepository>();
        _service = new ExpenseService(_expenseRepo.Object, _categoryRepo.Object);
    }

    [Fact]
    public async Task GetSummaryAsync_CallsAggregatedMethodOnce()
    {
        _expenseRepo
            .Setup(r => r.GetAggregatedSummaryAsync(
                It.IsAny<DateTime>(), It.IsAny<DateTime>(),
                It.IsAny<DateTime>(), It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExpenseSummary(1, 2, 3, 4, 5, 6));

        var result = await _service.GetSummaryAsync();

        result.Should().NotBeNull();
        result.TotalByDay.Should().Be(1);
        result.TotalByWeek.Should().Be(2);
        result.TotalByMonth.Should().Be(3);
        result.TotalByYear.Should().Be(4);
        result.TotalFixed.Should().Be(5);
        result.TotalVariable.Should().Be(6);

        _expenseRepo.Verify(r => r.GetAggregatedSummaryAsync(
            It.IsAny<DateTime>(), It.IsAny<DateTime>(),
            It.IsAny<DateTime>(), It.IsAny<DateTime>(),
            It.IsAny<CancellationToken>()), Times.Once);

        _expenseRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetSummaryAsync_WithNoData_ReturnsAllZeros()
    {
        _expenseRepo
            .Setup(r => r.GetAggregatedSummaryAsync(
                It.IsAny<DateTime>(), It.IsAny<DateTime>(),
                It.IsAny<DateTime>(), It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExpenseSummary(0, 0, 0, 0, 0, 0));

        var result = await _service.GetSummaryAsync();

        result.TotalByDay.Should().Be(0);
        result.TotalByWeek.Should().Be(0);
        result.TotalByMonth.Should().Be(0);
        result.TotalByYear.Should().Be(0);
        result.TotalFixed.Should().Be(0);
        result.TotalVariable.Should().Be(0);

        _expenseRepo.Verify(r => r.GetAggregatedSummaryAsync(
            It.IsAny<DateTime>(), It.IsAny<DateTime>(),
            It.IsAny<DateTime>(), It.IsAny<DateTime>(),
            It.IsAny<CancellationToken>()), Times.Once);

        _expenseRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        _expenseRepo
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expense?)null);

        var result = await _service.GetByIdAsync(99);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_MapsToResponse_WhenFound()
    {
        var expense = new Expense
        {
            Id = 1,
            Amount = 150.50m,
            Date = new DateTime(2026, 6, 12, 10, 0, 0, DateTimeKind.Utc),
            Description = "Test expense",
            Type = ExpenseType.Variable,
            CategoryId = 5,
            Category = new Category { Id = 5, Name = "Food" }
        };

        _expenseRepo
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expense);

        var result = await _service.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Amount.Should().Be(150.50m);
        result.Description.Should().Be("Test expense");
        result.Type.Should().Be(ExpenseType.Variable);
        result.CategoryId.Should().Be(5);
        result.CategoryName.Should().Be("Food");
    }

    [Fact]
    public async Task GetTotalByDayAsync_DelegatesToRepo()
    {
        var date = new DateTime(2026, 6, 12, 14, 30, 0, DateTimeKind.Utc);
        _expenseRepo
            .Setup(r => r.GetTotalByDateRangeAsync(
                It.IsAny<DateTime>(), It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(500m);

        var result = await _service.GetTotalByDayAsync(date);

        result.Should().Be(500m);

        _expenseRepo.Verify(r => r.GetTotalByDateRangeAsync(
            new DateTime(2026, 6, 12, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 6, 13, 0, 0, 0, DateTimeKind.Utc),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenCategoryNotFound()
    {
        _categoryRepo
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var request = new CreateExpenseRequest(100m, DateTime.UtcNow, null, ExpenseType.Fixed, 999);

        await _service.Invoking(s => s.CreateAsync(request))
            .Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*999*");

        _expenseRepo.Verify(r => r.CreateAsync(It.IsAny<Expense>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_SendsUtcDate()
    {
        var category = new Category { Id = 1, Name = "Food" };
        _categoryRepo
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _expenseRepo
            .Setup(r => r.CreateAsync(It.IsAny<Expense>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expense e, CancellationToken ct) =>
            {
                e.Id = 1;
                e.Category = category;
                return e;
            });

        var localDate = new DateTime(2026, 6, 12, 10, 0, 0, DateTimeKind.Local);
        var request = new CreateExpenseRequest(100m, localDate, null, ExpenseType.Fixed, 1);

        await _service.CreateAsync(request);

        _expenseRepo.Verify(r => r.CreateAsync(
            It.Is<Expense>(e => e.Date.Kind == DateTimeKind.Utc),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AllMethods_PropagateCancellationToken()
    {
        var cts = new CancellationTokenSource();
        var token = cts.Token;

        _expenseRepo
            .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Expense>([], 0, 1, 20, 0));

        _expenseRepo
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expense?)null);

        _expenseRepo
            .Setup(r => r.GetTotalByDateRangeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0m);

        _expenseRepo
            .Setup(r => r.GetTotalByTypeAsync(It.IsAny<ExpenseType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0m);

        await _service.GetAllAsync(page: 1, pageSize: 20, cancellationToken: token);
        await _service.GetByIdAsync(1, cancellationToken: token);
        await _service.GetTotalByDayAsync(DateTime.UtcNow, cancellationToken: token);
        await _service.GetTotalFixedAsync(cancellationToken: token);

        _expenseRepo.Verify(r => r.GetAllAsync(1, 20, token), Times.Once);
        _expenseRepo.Verify(r => r.GetByIdAsync(1, token), Times.Once);
        _expenseRepo.Verify(r => r.GetTotalByDateRangeAsync(
            It.IsAny<DateTime>(), It.IsAny<DateTime>(), token), Times.Once);
        _expenseRepo.Verify(r => r.GetTotalByTypeAsync(ExpenseType.Fixed, token), Times.Once);
    }
}
