using Microsoft.EntityFrameworkCore;
using MoneyControl.Data;
using MoneyControl.Models;

namespace MoneyControl.Repositories;

public class ExpenseRepository(AppDbContext context) : IExpenseRepository
{
    public async Task<IEnumerable<Expense>> GetAllAsync()
    {
        return await context.Expenses.Include(e => e.Category).ToListAsync();
    }

    public async Task<Expense?> GetByIdAsync(int id)
    {
        return await context.Expenses.Include(e => e.Category).FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Expense> CreateAsync(Expense expense)
    {
        context.Expenses.Add(expense);
        await context.SaveChangesAsync();
        return expense;
    }

    public async Task<Expense?> UpdateAsync(Expense expense)
    {
        var existing = await context.Expenses.FindAsync(expense.Id);
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
