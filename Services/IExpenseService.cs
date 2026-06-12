using MoneyControl.DTOs;

namespace MoneyControl.Services;

public interface IExpenseService
{
    Task<PagedResponse<ExpenseResponse>> GetAllAsync(int page = 1, int pageSize = 20);
    Task<ExpenseResponse?> GetByIdAsync(int id);
    Task<PagedResponse<ExpenseResponse>> GetByCategoryAsync(int categoryId, int page = 1, int pageSize = 20);
    Task<ExpenseResponse> CreateAsync(CreateExpenseRequest request);
    Task<ExpenseResponse?> UpdateAsync(int id, UpdateExpenseRequest request);
    Task<bool> DeleteAsync(int id);
    Task<decimal> GetTotalByDayAsync(DateTime date);
    Task<decimal> GetTotalByWeekAsync(DateTime date);
    Task<decimal> GetTotalByMonthAsync(int year, int month);
    Task<decimal> GetTotalByYearAsync(int year);
    Task<decimal> GetTotalFixedAsync();
    Task<decimal> GetTotalVariableAsync();
    Task<ExpenseSummaryResponse> GetSummaryAsync();
}
