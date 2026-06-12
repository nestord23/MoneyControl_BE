using Microsoft.EntityFrameworkCore;
using MoneyControl.Data;
using MoneyControl.Models;

namespace MoneyControl.Repositories;

public class ExpenseRepository(AppDbContext context) : IExpenseRepository
{
    public async Task<PagedResult<Expense>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var query = context.Expenses.AsNoTracking().Include(e => e.Category);
        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Expense>(items, totalCount, page, pageSize, totalPages);
    }

    public async Task<Expense?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Expenses
            .AsNoTracking()
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<PagedResult<Expense>> GetByCategoryAsync(int categoryId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var query = context.Expenses
            .AsNoTracking()
            .Include(e => e.Category)
            .Where(e => e.CategoryId == categoryId);

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Expense>(items, totalCount, page, pageSize, totalPages);
    }

    public async Task<PagedResult<Expense>> GetByDateRangeAsync(DateTime start, DateTime end, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var query = context.Expenses
            .AsNoTracking()
            .Include(e => e.Category)
            .Where(e => e.Date >= start && e.Date < end);

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Expense>(items, totalCount, page, pageSize, totalPages);
    }

    public async Task<PagedResult<Expense>> GetByTypeAsync(ExpenseType type, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var query = context.Expenses
            .AsNoTracking()
            .Include(e => e.Category)
            .Where(e => e.Type == type);

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Expense>(items, totalCount, page, pageSize, totalPages);
    }

    public async Task<decimal> GetTotalByDateRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        return await context.Expenses
            .AsNoTracking()
            .Where(e => e.Date >= start && e.Date < end)
            .SumAsync(e => e.Amount, cancellationToken);
    }

    public async Task<decimal> GetTotalByTypeAsync(ExpenseType type, CancellationToken cancellationToken = default)
    {
        return await context.Expenses
            .AsNoTracking()
            .Where(e => e.Type == type)
            .SumAsync(e => e.Amount, cancellationToken);
    }

    public async Task<ExpenseSummary> GetAggregatedSummaryAsync(DateTime dayStart, DateTime weekStart, DateTime monthStart, DateTime yearStart, CancellationToken cancellationToken = default)
    {
        var yearEnd = yearStart.AddYears(1);

        var result = await context.Expenses
            .AsNoTracking()
            .Where(e => e.Date >= yearStart && e.Date < yearEnd)
            .GroupBy(e => 1)
            .Select(g => new ExpenseSummary(
                g.Where(e => e.Date >= dayStart && e.Date < dayStart.AddDays(1)).Sum(e => e.Amount),
                g.Where(e => e.Date >= weekStart && e.Date < weekStart.AddDays(7)).Sum(e => e.Amount),
                g.Where(e => e.Date >= monthStart && e.Date < monthStart.AddMonths(1)).Sum(e => e.Amount),
                g.Where(e => e.Date >= yearStart && e.Date < yearEnd).Sum(e => e.Amount),
                g.Where(e => e.Type == ExpenseType.Fixed).Sum(e => e.Amount),
                g.Where(e => e.Type == ExpenseType.Variable).Sum(e => e.Amount)
            ))
            .FirstOrDefaultAsync(cancellationToken);

        return result ?? new ExpenseSummary(0, 0, 0, 0, 0, 0);
    }

    public async Task<bool> ExistsByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        return await context.Expenses
            .AsNoTracking()
            .AnyAsync(e => e.CategoryId == categoryId, cancellationToken);
    }

    public async Task<Expense> CreateAsync(Expense expense, CancellationToken cancellationToken = default)
    {
        context.Expenses.Add(expense);
        await context.SaveChangesAsync(cancellationToken);

        await context.Entry(expense).Reference(e => e.Category).LoadAsync(cancellationToken);
        return expense;
    }

    public async Task<Expense?> UpdateAsync(Expense expense, CancellationToken cancellationToken = default)
    {
        var existing = await context.Expenses
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.Id == expense.Id, cancellationToken);
        if (existing is null)
            return null;

        context.Entry(existing).CurrentValues.SetValues(expense);
        await context.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var expense = await context.Expenses.FindAsync([id], cancellationToken);
        if (expense is null)
            return false;

        context.Expenses.Remove(expense);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
