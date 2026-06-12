using Microsoft.EntityFrameworkCore;
using MoneyControl.Data;
using MoneyControl.Models;

namespace MoneyControl.Repositories;

public class IncomeRepository(AppDbContext context) : IIncomeRepository
{
    public async Task<PagedResult<Income>> GetAllAsync(int page = 1, int pageSize = 20)
    {
        var query = context.Incomes.AsNoTracking();
        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Income>(items, totalCount, page, pageSize, totalPages);
    }

    public async Task<Income?> GetByIdAsync(int id)
    {
        return await context.Incomes.FindAsync(id);
    }

    public async Task<PagedResult<Income>> GetByDateRangeAsync(DateTime start, DateTime end, int page = 1, int pageSize = 20)
    {
        var query = context.Incomes
            .AsNoTracking()
            .Where(i => i.Date >= start && i.Date < end);

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Income>(items, totalCount, page, pageSize, totalPages);
    }

    public async Task<decimal> GetTotalByDateRangeAsync(DateTime start, DateTime end)
    {
        return await context.Incomes
            .AsNoTracking()
            .Where(i => i.Date >= start && i.Date < end)
            .SumAsync(i => i.Amount);
    }

    public async Task<Income> CreateAsync(Income income)
    {
        context.Incomes.Add(income);
        await context.SaveChangesAsync();
        return income;
    }

    public async Task<Income?> UpdateAsync(Income income)
    {
        var existing = await context.Incomes.FindAsync(income.Id);
        if (existing is null)
            return null;

        context.Entry(existing).CurrentValues.SetValues(income);
        await context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var income = await context.Incomes.FindAsync(id);
        if (income is null)
            return false;

        context.Incomes.Remove(income);
        await context.SaveChangesAsync();
        return true;
    }
}
