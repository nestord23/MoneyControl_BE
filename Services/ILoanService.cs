using MoneyControl.DTOs;
using MoneyControl.Models;

namespace MoneyControl.Services;

public interface ILoanService
{
    Task<PagedResult<LoanResponse>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<LoanResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PagedResult<LoanResponse>> GetPendingLoansAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<PagedResult<LoanResponse>> GetPaidLoansAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<LoanResponse> CreateAsync(CreateLoanRequest request, CancellationToken cancellationToken = default);
    Task<LoanResponse?> UpdateAsync(int id, UpdateLoanRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<LoanResponse?> MarkAsPaidAsync(int id, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalPendingAmountAsync(CancellationToken cancellationToken = default);
    Task<decimal> GetTotalPaidAmountAsync(CancellationToken cancellationToken = default);
    Task<LoanSummaryResponse> GetSummaryAsync(CancellationToken cancellationToken = default);
}
