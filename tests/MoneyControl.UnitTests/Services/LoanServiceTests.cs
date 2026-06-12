using FluentAssertions;
using Moq;
using MoneyControl.DTOs;
using MoneyControl.Models;
using MoneyControl.Repositories;
using MoneyControl.Services;

namespace MoneyControl.UnitTests.Services;

public class LoanServiceTests
{
    private readonly Mock<ILoanRepository> _repo;
    private readonly LoanService _service;

    public LoanServiceTests()
    {
        _repo = new Mock<ILoanRepository>();
        _service = new LoanService(_repo.Object);
    }

    [Fact]
    public async Task MarkAsPaidAsync_ReturnsNull_WhenLoanNotFound()
    {
        _repo
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Loan?)null);

        var result = await _service.MarkAsPaidAsync(99);

        result.Should().BeNull();
    }

    [Fact]
    public async Task MarkAsPaidAsync_Throws_WhenAlreadyPaid()
    {
        var loan = new Loan
        {
            Id = 1,
            Amount = 500m,
            LenderName = "Bank",
            Date = DateTime.UtcNow,
            Status = LoanStatus.Paid
        };

        _repo
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(loan);

        await _service.Invoking(s => s.MarkAsPaidAsync(1))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already paid*");

        _repo.Verify(r => r.UpdateAsync(It.IsAny<Loan>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task MarkAsPaidAsync_SendsUpdate_WhenPending()
    {
        var loan = new Loan
        {
            Id = 1,
            Amount = 500m,
            LenderName = "Bank",
            Date = DateTime.UtcNow,
            Status = LoanStatus.Pending
        };

        _repo
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(loan);

        _repo
            .Setup(r => r.UpdateAsync(It.IsAny<Loan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Loan l, CancellationToken ct) =>
            {
                l.Status = LoanStatus.Paid;
                return l;
            });

        var result = await _service.MarkAsPaidAsync(1);

        result.Should().NotBeNull();
        result!.Status.Should().Be(LoanStatus.Paid);

        _repo.Verify(r => r.UpdateAsync(
            It.Is<Loan>(l => l.Status == LoanStatus.Paid),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_SetsStatusPending()
    {
        var request = new CreateLoanRequest(1000m, "Bank", DateTime.UtcNow);

        _repo
            .Setup(r => r.CreateAsync(It.IsAny<Loan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Loan l, CancellationToken ct) =>
            {
                l.Id = 1;
                return l;
            });

        var result = await _service.CreateAsync(request);

        _repo.Verify(r => r.CreateAsync(
            It.Is<Loan>(l => l.Status == LoanStatus.Pending),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetSummaryAsync_CallsRepoOnce()
    {
        _repo
            .Setup(r => r.GetSummaryAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LoanSummary(1000m, 500m, 2, 1));

        var result = await _service.GetSummaryAsync();

        result.Should().NotBeNull();
        result.TotalPending.Should().Be(1000m);
        result.TotalPaid.Should().Be(500m);
        result.PendingCount.Should().Be(2);
        result.PaidCount.Should().Be(1);

        _repo.Verify(r => r.GetSummaryAsync(It.IsAny<CancellationToken>()), Times.Once);
        _repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task AllMethods_PropagateCancellationToken()
    {
        var cts = new CancellationTokenSource();
        var token = cts.Token;

        _repo
            .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Loan>([], 0, 1, 20, 0));

        _repo
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Loan?)null);

        _repo
            .Setup(r => r.GetByStatusAsync(It.IsAny<LoanStatus>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Loan>([], 0, 1, 20, 0));

        _repo
            .Setup(r => r.GetTotalByStatusAsync(It.IsAny<LoanStatus>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0m);

        await _service.GetAllAsync(page: 1, pageSize: 20, cancellationToken: token);
        await _service.GetByIdAsync(1, cancellationToken: token);
        await _service.GetPendingLoansAsync(page: 1, pageSize: 20, cancellationToken: token);
        await _service.GetTotalPendingAmountAsync(cancellationToken: token);

        _repo.Verify(r => r.GetAllAsync(1, 20, token), Times.Once);
        _repo.Verify(r => r.GetByIdAsync(1, token), Times.Once);
        _repo.Verify(r => r.GetByStatusAsync(LoanStatus.Pending, 1, 20, token), Times.Once);
        _repo.Verify(r => r.GetTotalByStatusAsync(LoanStatus.Pending, token), Times.Once);
    }
}
