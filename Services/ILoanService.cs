using MoneyControl.DTOs;

namespace MoneyControl.Services;

public interface ILoanService
{
    Task<PagedResponse<LoanResponse>> GetAllAsync(int page = 1, int pageSize = 20);
    Task<LoanResponse?> GetByIdAsync(int id);
    Task<PagedResponse<LoanResponse>> GetPendingLoansAsync(int page = 1, int pageSize = 20);
    Task<PagedResponse<LoanResponse>> GetPaidLoansAsync(int page = 1, int pageSize = 20);
    Task<LoanResponse> CreateAsync(CreateLoanRequest request);
    Task<LoanResponse?> UpdateAsync(int id, UpdateLoanRequest request);
    Task<bool> DeleteAsync(int id);
    Task<LoanResponse?> MarkAsPaidAsync(int id);
    Task<decimal> GetTotalPendingAmountAsync();
    Task<decimal> GetTotalPaidAmountAsync();
}
