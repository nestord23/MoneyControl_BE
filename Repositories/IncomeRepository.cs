using Microsoft.EntityFrameworkCore;
using MoneyControl.Data;
using MoneyControl.Models;

namespace MoneyControl.Repositories;

public class IncomeRepository(AppDbContext context) : IIncomeRepository
{
    public async Task<IEnumerable<Income>> GetAllAsync()
    {
        return await context.Incomes.ToListAsync();
    }

    public async Task<Income?> GetByIdAsync(int id)
    {
        return await context.Incomes.FindAsync(id);
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
