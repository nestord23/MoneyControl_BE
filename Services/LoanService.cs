using MoneyControl.DTOs;
using MoneyControl.Models;
using MoneyControl.Repositories;

namespace MoneyControl.Services;

public class LoanService(ILoanRepository repository) : ILoanService
{
    public async Task<PagedResponse<LoanResponse>> GetAllAsync(int page = 1, int pageSize = 20)
    {
        var result = await repository.GetAllAsync(page, pageSize);
        return new PagedResponse<LoanResponse>(
            result.Items.Select(MapToResponse),
            result.TotalCount,
            result.Page,
            result.PageSize,
            result.TotalPages
        );
    }

    public async Task<LoanResponse?> GetByIdAsync(int id)
    {
        var loan = await repository.GetByIdAsync(id);
        return loan is null ? null : MapToResponse(loan);
    }

    public async Task<PagedResponse<LoanResponse>> GetPendingLoansAsync(int page = 1, int pageSize = 20)
    {
        var result = await repository.GetByStatusAsync(LoanStatus.Pending, page, pageSize);
        return new PagedResponse<LoanResponse>(
            result.Items.Select(MapToResponse),
            result.TotalCount,
            result.Page,
            result.PageSize,
            result.TotalPages
        );
    }

    public async Task<PagedResponse<LoanResponse>> GetPaidLoansAsync(int page = 1, int pageSize = 20)
    {
        var result = await repository.GetByStatusAsync(LoanStatus.Paid, page, pageSize);
        return new PagedResponse<LoanResponse>(
            result.Items.Select(MapToResponse),
            result.TotalCount,
            result.Page,
            result.PageSize,
            result.TotalPages
        );
    }

    public async Task<LoanResponse> CreateAsync(CreateLoanRequest request)
    {
        var loan = new Loan
        {
            Amount = request.Amount,
            LenderName = request.LenderName,
            Date = DateTime.SpecifyKind(request.Date, DateTimeKind.Utc),
            Status = LoanStatus.Pending
        };

        var created = await repository.CreateAsync(loan);
        return MapToResponse(created);
    }

    public async Task<LoanResponse?> UpdateAsync(int id, UpdateLoanRequest request)
    {
        var loan = new Loan
        {
            Id = id,
            Amount = request.Amount,
            LenderName = request.LenderName,
            Date = DateTime.SpecifyKind(request.Date, DateTimeKind.Utc),
            Status = request.Status
        };

        var updated = await repository.UpdateAsync(loan);
        return updated is null ? null : MapToResponse(updated);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await repository.DeleteAsync(id);
    }

    public async Task<LoanResponse?> MarkAsPaidAsync(int id)
    {
        var existing = await repository.GetByIdAsync(id);
        if (existing is null)
            return null;

        if (existing.Status == LoanStatus.Paid)
            throw new InvalidOperationException($"Loan with id {id} is already paid.");

        existing.Status = LoanStatus.Paid;
        var updated = await repository.UpdateAsync(existing);
        return updated is null ? null : MapToResponse(updated);
    }

    public async Task<decimal> GetTotalPendingAmountAsync()
    {
        return await repository.GetTotalByStatusAsync(LoanStatus.Pending);
    }

    public async Task<decimal> GetTotalPaidAmountAsync()
    {
        return await repository.GetTotalByStatusAsync(LoanStatus.Paid);
    }

    private static LoanResponse MapToResponse(Loan loan)
    {
        return new LoanResponse(loan.Id, loan.Amount, loan.LenderName, loan.Date, loan.Status);
    }
}
