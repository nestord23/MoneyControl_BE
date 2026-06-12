using Microsoft.EntityFrameworkCore;
using MoneyControl.Data;
using MoneyControl.Models;

namespace MoneyControl.Repositories;

public class LoanRepository(AppDbContext context) : ILoanRepository
{
    public async Task<PagedResult<Loan>> GetAllAsync(int page = 1, int pageSize = 20)
    {
        var query = context.Loans.AsNoTracking();
        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Loan>(items, totalCount, page, pageSize, totalPages);
    }

    public async Task<Loan?> GetByIdAsync(int id)
    {
        return await context.Loans.FindAsync(id);
    }

    public async Task<PagedResult<Loan>> GetByStatusAsync(LoanStatus status, int page = 1, int pageSize = 20)
    {
        var query = context.Loans
            .AsNoTracking()
            .Where(l => l.Status == status);

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Loan>(items, totalCount, page, pageSize, totalPages);
    }

    public async Task<decimal> GetTotalByStatusAsync(LoanStatus status)
    {
        return await context.Loans
            .AsNoTracking()
            .Where(l => l.Status == status)
            .SumAsync(l => l.Amount);
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
