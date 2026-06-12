using MoneyControl.DTOs;
using MoneyControl.Models;
using MoneyControl.Repositories;

namespace MoneyControl.Services;

public class IncomeService(IIncomeRepository repository) : IIncomeService
{
    public async Task<PagedResponse<IncomeResponse>> GetAllAsync(int page = 1, int pageSize = 20)
    {
        var result = await repository.GetAllAsync(page, pageSize);
        return new PagedResponse<IncomeResponse>(
            result.Items.Select(MapToResponse),
            result.TotalCount,
            result.Page,
            result.PageSize,
            result.TotalPages
        );
    }

    public async Task<IncomeResponse?> GetByIdAsync(int id)
    {
        var income = await repository.GetByIdAsync(id);
        return income is null ? null : MapToResponse(income);
    }

    public async Task<IncomeResponse> CreateAsync(CreateIncomeRequest request)
    {
        var income = new Income
        {
            Amount = request.Amount,
            Date = DateTime.SpecifyKind(request.Date, DateTimeKind.Utc),
            Description = request.Description
        };

        var created = await repository.CreateAsync(income);
        return MapToResponse(created);
    }

    public async Task<IncomeResponse?> UpdateAsync(int id, UpdateIncomeRequest request)
    {
        var income = new Income
        {
            Id = id,
            Amount = request.Amount,
            Date = DateTime.SpecifyKind(request.Date, DateTimeKind.Utc),
            Description = request.Description
        };

        var updated = await repository.UpdateAsync(income);
        return updated is null ? null : MapToResponse(updated);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await repository.DeleteAsync(id);
    }

    public async Task<decimal> GetTotalByDayAsync(DateTime date)
    {
        var start = date.Date;
        var end = start.AddDays(1);
        return await repository.GetTotalByDateRangeAsync(start, end);
    }

    public async Task<decimal> GetTotalByWeekAsync(DateTime date)
    {
        var daysSinceMonday = (int)date.DayOfWeek - 1;
        if (daysSinceMonday < 0) daysSinceMonday += 7;
        var monday = date.Date.AddDays(-daysSinceMonday);
        var nextMonday = monday.AddDays(7);
        return await repository.GetTotalByDateRangeAsync(monday, nextMonday);
    }

    public async Task<decimal> GetTotalByMonthAsync(int year, int month)
    {
        var start = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var end = start.AddMonths(1);
        return await repository.GetTotalByDateRangeAsync(start, end);
    }

    public async Task<decimal> GetTotalByYearAsync(int year)
    {
        var start = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var end = start.AddYears(1);
        return await repository.GetTotalByDateRangeAsync(start, end);
    }

    public async Task<IncomeSummaryResponse> GetSummaryAsync()
    {
        var now = DateTime.UtcNow;
        var dayStart = now.Date;
        var weekStart = now.Date.AddDays(-(int)now.DayOfWeek + 1);
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var yearStart = new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var totalByDay = await repository.GetTotalByDateRangeAsync(dayStart, dayStart.AddDays(1));
        var totalByWeek = await repository.GetTotalByDateRangeAsync(weekStart, weekStart.AddDays(7));
        var totalByMonth = await repository.GetTotalByDateRangeAsync(monthStart, monthStart.AddMonths(1));
        var totalByYear = await repository.GetTotalByDateRangeAsync(yearStart, yearStart.AddYears(1));

        return new IncomeSummaryResponse(totalByDay, totalByWeek, totalByMonth, totalByYear);
    }

    private static IncomeResponse MapToResponse(Income income)
    {
        return new IncomeResponse(income.Id, income.Amount, income.Date, income.Description);
    }
}
