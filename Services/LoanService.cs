using MoneyControl.DTOs;
using MoneyControl.Helpers;
using MoneyControl.Models;
using MoneyControl.Repositories;

namespace MoneyControl.Services;

public class LoanService(ILoanRepository repository) : ILoanService
{
    public async Task<PagedResult<LoanResponse>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await repository.GetAllAsync(page, pageSize, cancellationToken);
        return result.Map(MapToResponse);
    }

    public async Task<LoanResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var loan = await repository.GetByIdAsync(id, cancellationToken);
        return loan is null ? null : MapToResponse(loan);
    }

    public async Task<PagedResult<LoanResponse>> GetPendingLoansAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await repository.GetByStatusAsync(LoanStatus.Pending, page, pageSize, cancellationToken);
        return result.Map(MapToResponse);
    }

    public async Task<PagedResult<LoanResponse>> GetPaidLoansAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await repository.GetByStatusAsync(LoanStatus.Paid, page, pageSize, cancellationToken);
        return result.Map(MapToResponse);
    }

    public async Task<LoanResponse> CreateAsync(CreateLoanRequest request, CancellationToken cancellationToken = default)
    {
        var loan = new Loan
        {
            Amount = request.Amount,
            LenderName = request.LenderName,
            Date = ToUtc(request.Date),
            Status = LoanStatus.Pending
        };

        var created = await repository.CreateAsync(loan, cancellationToken);
        return MapToResponse(created);
    }

    public async Task<LoanResponse?> UpdateAsync(int id, UpdateLoanRequest request, CancellationToken cancellationToken = default)
    {
        var loan = new Loan
        {
            Id = id,
            Amount = request.Amount,
            LenderName = request.LenderName,
            Date = ToUtc(request.Date),
            Status = request.Status
        };

        var updated = await repository.UpdateAsync(loan, cancellationToken);
        return updated is null ? null : MapToResponse(updated);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return await repository.DeleteAsync(id, cancellationToken);
    }

    public async Task<LoanResponse?> MarkAsPaidAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
            return null;

        if (existing.Status == LoanStatus.Paid)
            throw new InvalidOperationException($"Loan with id {id} is already paid.");

        existing.Status = LoanStatus.Paid;
        var updated = await repository.UpdateAsync(existing, cancellationToken);
        return updated is null ? null : MapToResponse(updated);
    }

    public async Task<decimal> GetTotalPendingAmountAsync(CancellationToken cancellationToken = default)
    {
        return await repository.GetTotalByStatusAsync(LoanStatus.Pending, cancellationToken);
    }

    public async Task<decimal> GetTotalPaidAmountAsync(CancellationToken cancellationToken = default)
    {
        return await repository.GetTotalByStatusAsync(LoanStatus.Paid, cancellationToken);
    }

    public async Task<LoanSummaryResponse> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var summary = await repository.GetSummaryAsync(cancellationToken);
        return new LoanSummaryResponse(summary.TotalPending, summary.TotalPaid, summary.PendingCount, summary.PaidCount);
    }

    private static DateTime ToUtc(DateTime date) => DateTimeHelper.ToUtc(date);

    private static LoanResponse MapToResponse(Loan loan)
    {
        return new LoanResponse(loan.Id, loan.Amount, loan.LenderName, loan.Date, loan.Status);
    }
}
