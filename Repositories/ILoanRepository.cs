using MoneyControl.Models;

namespace MoneyControl.Repositories;

public interface ILoanRepository
{
    Task<PagedResult<Loan>> GetAllAsync(int page = 1, int pageSize = 20);
    Task<Loan?> GetByIdAsync(int id);
    Task<PagedResult<Loan>> GetByStatusAsync(LoanStatus status, int page = 1, int pageSize = 20);
    Task<decimal> GetTotalByStatusAsync(LoanStatus status);
    Task<Loan> CreateAsync(Loan loan);
    Task<Loan?> UpdateAsync(Loan loan);
    Task<bool> DeleteAsync(int id);
}
