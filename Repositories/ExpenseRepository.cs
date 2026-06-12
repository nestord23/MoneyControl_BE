using Microsoft.EntityFrameworkCore;
using MoneyControl.Data;
using MoneyControl.Models;

namespace MoneyControl.Repositories;

public class ExpenseRepository(AppDbContext context) : IExpenseRepository
{
    public async Task<PagedResult<Expense>> GetAllAsync(int page = 1, int pageSize = 20)
    {
        var query = context.Expenses.AsNoTracking().Include(e => e.Category);
        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Expense>(items, totalCount, page, pageSize, totalPages);
    }

    public async Task<Expense?> GetByIdAsync(int id)
    {
        return await context.Expenses
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<PagedResult<Expense>> GetByCategoryAsync(int categoryId, int page = 1, int pageSize = 20)
    {
        var query = context.Expenses
            .AsNoTracking()
            .Include(e => e.Category)
            .Where(e => e.CategoryId == categoryId);

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Expense>(items, totalCount, page, pageSize, totalPages);
    }

    public async Task<PagedResult<Expense>> GetByDateRangeAsync(DateTime start, DateTime end, int page = 1, int pageSize = 20)
    {
        var query = context.Expenses
            .AsNoTracking()
            .Include(e => e.Category)
            .Where(e => e.Date >= start && e.Date < end);

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Expense>(items, totalCount, page, pageSize, totalPages);
    }

    public async Task<PagedResult<Expense>> GetByTypeAsync(ExpenseType type, int page = 1, int pageSize = 20)
    {
        var query = context.Expenses
            .AsNoTracking()
            .Include(e => e.Category)
            .Where(e => e.Type == type);

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Expense>(items, totalCount, page, pageSize, totalPages);
    }

    public async Task<decimal> GetTotalByDateRangeAsync(DateTime start, DateTime end)
    {
        return await context.Expenses
            .AsNoTracking()
            .Where(e => e.Date >= start && e.Date < end)
            .SumAsync(e => e.Amount);
    }

    public async Task<decimal> GetTotalByTypeAsync(ExpenseType type)
    {
        return await context.Expenses
            .AsNoTracking()
            .Where(e => e.Type == type)
            .SumAsync(e => e.Amount);
    }

    public async Task<Expense> CreateAsync(Expense expense)
    {
        context.Expenses.Add(expense);
        await context.SaveChangesAsync();

        return await context.Expenses
            .Include(e => e.Category)
            .FirstAsync(e => e.Id == expense.Id);
    }

    public async Task<Expense?> UpdateAsync(Expense expense)
    {
        var existing = await context.Expenses
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.Id == expense.Id);
        if (existing is null)
            return null;

        context.Entry(existing).CurrentValues.SetValues(expense);
        await context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var expense = await context.Expenses.FindAsync(id);
        if (expense is null)
            return false;

        context.Expenses.Remove(expense);
        await context.SaveChangesAsync();
        return true;
    }
}
