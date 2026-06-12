using MoneyControl.Models;

namespace MoneyControl.Repositories;

public interface IIncomeRepository
{
    Task<PagedResult<Income>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<Income?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PagedResult<Income>> GetByDateRangeAsync(DateTime start, DateTime end, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalByDateRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
    Task<IncomeSummary> GetAggregatedSummaryAsync(DateTime dayStart, DateTime weekStart, DateTime monthStart, DateTime yearStart, CancellationToken cancellationToken = default);
    Task<Income> CreateAsync(Income income, CancellationToken cancellationToken = default);
    Task<Income?> UpdateAsync(Income income, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
