using System.Globalization;
using MoneyControl.DTOs;
using MoneyControl.Models;
using MoneyControl.Repositories;

namespace MoneyControl.Services;

public class IncomeService(IIncomeRepository repository) : IIncomeService
{
    public async Task<PagedResult<IncomeResponse>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await repository.GetAllAsync(page, pageSize, cancellationToken);
        return result.Map(MapToResponse);
    }

    public async Task<IncomeResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var income = await repository.GetByIdAsync(id, cancellationToken);
        return income is null ? null : MapToResponse(income);
    }

    public async Task<IncomeResponse> CreateAsync(CreateIncomeRequest request, CancellationToken cancellationToken = default)
    {
        var income = new Income
        {
            Amount = request.Amount,
            Date = ToUtc(request.Date),
            Description = request.Description
        };

        var created = await repository.CreateAsync(income, cancellationToken);
        return MapToResponse(created);
    }

    public async Task<IncomeResponse?> UpdateAsync(int id, UpdateIncomeRequest request, CancellationToken cancellationToken = default)
    {
        var income = new Income
        {
            Id = id,
            Amount = request.Amount,
            Date = ToUtc(request.Date),
            Description = request.Description
        };

        var updated = await repository.UpdateAsync(income, cancellationToken);
        return updated is null ? null : MapToResponse(updated);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return await repository.DeleteAsync(id, cancellationToken);
    }

    public async Task<decimal> GetTotalByDayAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        var start = ToUtc(date.Date);
        var end = start.AddDays(1);
        return await repository.GetTotalByDateRangeAsync(start, end, cancellationToken);
    }

    public async Task<decimal> GetTotalByWeekAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        var weekStart = ToUtc(GetWeekStart(date));
        return await repository.GetTotalByDateRangeAsync(weekStart, weekStart.AddDays(7), cancellationToken);
    }

    public async Task<decimal> GetTotalByMonthAsync(int year, int month, CancellationToken cancellationToken = default)
    {
        var start = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var end = start.AddMonths(1);
        return await repository.GetTotalByDateRangeAsync(start, end, cancellationToken);
    }

    public async Task<decimal> GetTotalByYearAsync(int year, CancellationToken cancellationToken = default)
    {
        var start = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var end = start.AddYears(1);
        return await repository.GetTotalByDateRangeAsync(start, end, cancellationToken);
    }

    public async Task<IncomeSummaryResponse> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var dayStart = now.Date;
        var weekStart = GetWeekStart(now);
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var yearStart = new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var summary = await repository.GetAggregatedSummaryAsync(dayStart, weekStart, monthStart, yearStart, cancellationToken);

        return new IncomeSummaryResponse(summary.TotalByDay, summary.TotalByWeek, summary.TotalByMonth, summary.TotalByYear);
    }

    private static DateTime GetWeekStart(DateTime date)
    {
        var firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
        var diff = (7 + (int)date.DayOfWeek - (int)firstDayOfWeek) % 7;
        return date.Date.AddDays(-diff);
    }

    private static DateTime ToUtc(DateTime date)
    {
        return date.Kind switch
        {
            DateTimeKind.Utc => date,
            DateTimeKind.Local => date.ToUniversalTime(),
            _ => DateTime.SpecifyKind(date, DateTimeKind.Utc)
        };
    }

    private static IncomeResponse MapToResponse(Income income)
    {
        return new IncomeResponse(income.Id, income.Amount, income.Date, income.Description);
    }
}
