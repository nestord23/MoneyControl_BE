using Microsoft.EntityFrameworkCore;
using MoneyControl.Data;
using MoneyControl.Models;

namespace MoneyControl.Repositories;

public class IncomeRepository(AppDbContext context) : IIncomeRepository
{
    public async Task<PagedResult<Income>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var query = context.Incomes.AsNoTracking();
        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Income>(items, totalCount, page, pageSize, totalPages);
    }

    public async Task<Income?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Incomes
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<PagedResult<Income>> GetByDateRangeAsync(DateTime start, DateTime end, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var query = context.Incomes
            .AsNoTracking()
            .Where(i => i.Date >= start && i.Date < end);

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Income>(items, totalCount, page, pageSize, totalPages);
    }

    public async Task<decimal> GetTotalByDateRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        return await context.Incomes
            .AsNoTracking()
            .Where(i => i.Date >= start && i.Date < end)
            .SumAsync(i => i.Amount, cancellationToken);
    }

    public async Task<decimal[]> GetMonthlyTotalsAsync(int year, CancellationToken cancellationToken = default)
    {
        var start = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var end = start.AddYears(1);

        var raw = await context.Incomes
            .AsNoTracking()
            .Where(i => i.Date >= start && i.Date < end)
            .GroupBy(i => i.Date.Month)
            .Select(g => new { Month = g.Key, Total = g.Sum(i => i.Amount) })
            .ToListAsync(cancellationToken);

        var result = new decimal[12];
        foreach (var item in raw)
            result[item.Month - 1] = item.Total;
        return result;
    }

    public async Task<IncomeSummary> GetAggregatedSummaryAsync(DateTime dayStart, DateTime weekStart, DateTime monthStart, DateTime yearStart, CancellationToken cancellationToken = default)
    {
        var yearEnd = yearStart.AddYears(1);

        var result = await context.Incomes
            .AsNoTracking()
            .Where(i => i.Date >= yearStart && i.Date < yearEnd)
            .GroupBy(i => 1)
            .Select(g => new IncomeSummary(
                g.Where(i => i.Date >= dayStart && i.Date < dayStart.AddDays(1)).Sum(i => i.Amount),
                g.Where(i => i.Date >= weekStart && i.Date < weekStart.AddDays(7)).Sum(i => i.Amount),
                g.Where(i => i.Date >= monthStart && i.Date < monthStart.AddMonths(1)).Sum(i => i.Amount),
                g.Where(i => i.Date >= yearStart && i.Date < yearEnd).Sum(i => i.Amount)
            ))
            .FirstOrDefaultAsync(cancellationToken);

        return result ?? new IncomeSummary(0, 0, 0, 0);
    }

    public async Task<Income> CreateAsync(Income income, CancellationToken cancellationToken = default)
    {
        context.Incomes.Add(income);
        await context.SaveChangesAsync(cancellationToken);
        return income;
    }

    public async Task<Income?> UpdateAsync(Income income, CancellationToken cancellationToken = default)
    {
        var existing = await context.Incomes.FindAsync([income.Id], cancellationToken);
        if (existing is null)
            return null;

        context.Entry(existing).CurrentValues.SetValues(income);
        await context.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var income = await context.Incomes.FindAsync([id], cancellationToken);
        if (income is null)
            return false;

        context.Incomes.Remove(income);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
