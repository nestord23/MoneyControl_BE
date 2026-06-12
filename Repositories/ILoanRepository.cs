using MoneyControl.Models;

namespace MoneyControl.Repositories;

public interface ILoanRepository
{
    Task<PagedResult<Loan>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<Loan?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PagedResult<Loan>> GetByStatusAsync(LoanStatus status, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalByStatusAsync(LoanStatus status, CancellationToken cancellationToken = default);
    Task<LoanSummary> GetSummaryAsync(CancellationToken cancellationToken = default);
    Task<Loan> CreateAsync(Loan loan, CancellationToken cancellationToken = default);
    Task<Loan?> UpdateAsync(Loan loan, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
