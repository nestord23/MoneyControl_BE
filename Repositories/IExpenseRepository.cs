using MoneyControl.Models;

namespace MoneyControl.Repositories;

public interface IExpenseRepository
{
    Task<PagedResult<Expense>> GetAllAsync(int page = 1, int pageSize = 20);
    Task<Expense?> GetByIdAsync(int id);
    Task<PagedResult<Expense>> GetByCategoryAsync(int categoryId, int page = 1, int pageSize = 20);
    Task<PagedResult<Expense>> GetByDateRangeAsync(DateTime start, DateTime end, int page = 1, int pageSize = 20);
    Task<PagedResult<Expense>> GetByTypeAsync(ExpenseType type, int page = 1, int pageSize = 20);
    Task<decimal> GetTotalByDateRangeAsync(DateTime start, DateTime end);
    Task<decimal> GetTotalByTypeAsync(ExpenseType type);
    Task<Expense> CreateAsync(Expense expense);
    Task<Expense?> UpdateAsync(Expense expense);
    Task<bool> DeleteAsync(int id);
}
