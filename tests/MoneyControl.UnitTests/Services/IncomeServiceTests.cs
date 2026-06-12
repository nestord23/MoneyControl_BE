using FluentAssertions;
using Moq;
using MoneyControl.DTOs;
using MoneyControl.Models;
using MoneyControl.Repositories;
using MoneyControl.Services;

namespace MoneyControl.UnitTests.Services;

public class IncomeServiceTests
{
    private readonly Mock<IIncomeRepository> _repo;
    private readonly IncomeService _service;

    public IncomeServiceTests()
    {
        _repo = new Mock<IIncomeRepository>();
        _service = new IncomeService(_repo.Object);
    }

    [Fact]
    public async Task GetSummaryAsync_CallsAggregatedMethodOnce()
    {
        _repo
            .Setup(r => r.GetAggregatedSummaryAsync(
                It.IsAny<DateTime>(), It.IsAny<DateTime>(),
                It.IsAny<DateTime>(), It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IncomeSummary(100, 500, 2000, 24000));

        var result = await _service.GetSummaryAsync();

        result.TotalByDay.Should().Be(100);
        result.TotalByWeek.Should().Be(500);
        result.TotalByMonth.Should().Be(2000);
        result.TotalByYear.Should().Be(24000);

        _repo.Verify(r => r.GetAggregatedSummaryAsync(
            It.IsAny<DateTime>(), It.IsAny<DateTime>(),
            It.IsAny<DateTime>(), It.IsAny<DateTime>(),
            It.IsAny<CancellationToken>()), Times.Once);

        _repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateAsync_ConvertsDateToUtc()
    {
        var localDate = new DateTime(2026, 6, 12, 10, 0, 0, DateTimeKind.Local);
        var request = new CreateIncomeRequest(1000m, localDate, null);

        _repo
            .Setup(r => r.CreateAsync(It.IsAny<Income>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Income i, CancellationToken ct) =>
            {
                i.Id = 1;
                return i;
            });

        var result = await _service.CreateAsync(request);

        _repo.Verify(r => r.CreateAsync(
            It.Is<Income>(i => i.Date.Kind == DateTimeKind.Utc),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AllMethods_PropagateCancellationToken()
    {
        var cts = new CancellationTokenSource();
        var token = cts.Token;

        _repo
            .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Income>([], 0, 1, 20, 0));

        _repo
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Income?)null);

        _repo
            .Setup(r => r.GetTotalByDateRangeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0m);

        await _service.GetAllAsync(page: 1, pageSize: 20, cancellationToken: token);
        await _service.GetByIdAsync(1, cancellationToken: token);
        await _service.GetTotalByDayAsync(DateTime.UtcNow, cancellationToken: token);

        _repo.Verify(r => r.GetAllAsync(1, 20, token), Times.Once);
        _repo.Verify(r => r.GetByIdAsync(1, token), Times.Once);
        _repo.Verify(r => r.GetTotalByDateRangeAsync(
            It.IsAny<DateTime>(), It.IsAny<DateTime>(), token), Times.Once);
    }
}
