using MoneyControl.Models;

namespace MoneyControl.Repositories;

public interface IIncomeRepository
{
    Task<PagedResult<Income>> GetAllAsync(int page = 1, int pageSize = 20);
    Task<Income?> GetByIdAsync(int id);
    Task<PagedResult<Income>> GetByDateRangeAsync(DateTime start, DateTime end, int page = 1, int pageSize = 20);
    Task<decimal> GetTotalByDateRangeAsync(DateTime start, DateTime end);
    Task<Income> CreateAsync(Income income);
    Task<Income?> UpdateAsync(Income income);
    Task<bool> DeleteAsync(int id);
}
