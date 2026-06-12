using MoneyControl.DTOs;

namespace MoneyControl.Services;

public interface IIncomeService
{
    Task<PagedResponse<IncomeResponse>> GetAllAsync(int page = 1, int pageSize = 20);
    Task<IncomeResponse?> GetByIdAsync(int id);
    Task<IncomeResponse> CreateAsync(CreateIncomeRequest request);
    Task<IncomeResponse?> UpdateAsync(int id, UpdateIncomeRequest request);
    Task<bool> DeleteAsync(int id);
    Task<decimal> GetTotalByDayAsync(DateTime date);
    Task<decimal> GetTotalByWeekAsync(DateTime date);
    Task<decimal> GetTotalByMonthAsync(int year, int month);
    Task<decimal> GetTotalByYearAsync(int year);
    Task<IncomeSummaryResponse> GetSummaryAsync();
}
