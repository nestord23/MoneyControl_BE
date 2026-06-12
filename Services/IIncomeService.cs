using MoneyControl.DTOs;
using MoneyControl.Models;

namespace MoneyControl.Services;

public interface IIncomeService
{
    Task<PagedResult<IncomeResponse>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<IncomeResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IncomeResponse> CreateAsync(CreateIncomeRequest request, CancellationToken cancellationToken = default);
    Task<IncomeResponse?> UpdateAsync(int id, UpdateIncomeRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalByDayAsync(DateTime date, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalByWeekAsync(DateTime date, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalByMonthAsync(int year, int month, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalByYearAsync(int year, CancellationToken cancellationToken = default);
    Task<IncomeSummaryResponse> GetSummaryAsync(CancellationToken cancellationToken = default);
}
