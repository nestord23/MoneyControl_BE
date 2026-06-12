using MoneyControl.Models;

namespace MoneyControl.Repositories;

public interface IExpenseRepository
{
    Task<PagedResult<Expense>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<Expense?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PagedResult<Expense>> GetByCategoryAsync(int categoryId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<PagedResult<Expense>> GetByDateRangeAsync(DateTime start, DateTime end, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<PagedResult<Expense>> GetByTypeAsync(ExpenseType type, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalByDateRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalByTypeAsync(ExpenseType type, CancellationToken cancellationToken = default);
    Task<ExpenseSummary> GetAggregatedSummaryAsync(DateTime dayStart, DateTime weekStart, DateTime monthStart, DateTime yearStart, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
    Task<Expense> CreateAsync(Expense expense, CancellationToken cancellationToken = default);
    Task<Expense?> UpdateAsync(Expense expense, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
