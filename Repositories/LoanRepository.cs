using Microsoft.EntityFrameworkCore;
using MoneyControl.Data;
using MoneyControl.Models;

namespace MoneyControl.Repositories;

public class LoanRepository(AppDbContext context) : ILoanRepository
{
    public async Task<IEnumerable<Loan>> GetAllAsync()
    {
        return await context.Loans.ToListAsync();
    }

    public async Task<Loan?> GetByIdAsync(int id)
    {
        return await context.Loans.FindAsync(id);
    }

    public async Task<Loan> CreateAsync(Loan loan)
    {
        context.Loans.Add(loan);
        await context.SaveChangesAsync();
        return loan;
    }

    public async Task<Loan?> UpdateAsync(Loan loan)
    {
        var existing = await context.Loans.FindAsync(loan.Id);
        if (existing is null)
            return null;

        context.Entry(existing).CurrentValues.SetValues(loan);
        await context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var loan = await context.Loans.FindAsync(id);
        if (loan is null)
            return false;

        context.Loans.Remove(loan);
        await context.SaveChangesAsync();
        return true;
    }
}
