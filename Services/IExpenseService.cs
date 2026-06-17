using MoneyControl.DTOs;
using MoneyControl.Models;

namespace MoneyControl.Services;

public interface IExpenseService
{
    Task<PagedResult<ExpenseResponse>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<ExpenseResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PagedResult<ExpenseResponse>> GetByCategoryAsync(int categoryId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<ExpenseResponse> CreateAsync(CreateExpenseRequest request, CancellationToken cancellationToken = default);
    Task<ExpenseResponse?> UpdateAsync(int id, UpdateExpenseRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalByDayAsync(DateTime date, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalByWeekAsync(DateTime date, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalByMonthAsync(int year, int month, CancellationToken cancellationToken = default);
    Task<decimal[]> GetMonthlyTotalsAsync(int year, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalByYearAsync(int year, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalFixedAsync(CancellationToken cancellationToken = default);
    Task<decimal> GetTotalVariableAsync(CancellationToken cancellationToken = default);
    Task<ExpenseSummaryResponse> GetSummaryAsync(CancellationToken cancellationToken = default);
}
