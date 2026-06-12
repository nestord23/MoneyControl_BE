using Microsoft.EntityFrameworkCore;
using MoneyControl.Data;
using MoneyControl.Models;

namespace MoneyControl.Repositories;

public class LoanRepository(AppDbContext context) : ILoanRepository
{
    public async Task<PagedResult<Loan>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var query = context.Loans.AsNoTracking();
        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Loan>(items, totalCount, page, pageSize, totalPages);
    }

    public async Task<Loan?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Loans
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<PagedResult<Loan>> GetByStatusAsync(LoanStatus status, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var query = context.Loans
            .AsNoTracking()
            .Where(l => l.Status == status);

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Loan>(items, totalCount, page, pageSize, totalPages);
    }

    public async Task<decimal> GetTotalByStatusAsync(LoanStatus status, CancellationToken cancellationToken = default)
    {
        return await context.Loans
            .AsNoTracking()
            .Where(l => l.Status == status)
            .SumAsync(l => l.Amount, cancellationToken);
    }

    public async Task<LoanSummary> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var result = await context.Loans
            .AsNoTracking()
            .GroupBy(l => 1)
            .Select(g => new LoanSummary(
                g.Where(l => l.Status == LoanStatus.Pending).Sum(l => l.Amount),
                g.Where(l => l.Status == LoanStatus.Paid).Sum(l => l.Amount),
                g.Count(l => l.Status == LoanStatus.Pending),
                g.Count(l => l.Status == LoanStatus.Paid)
            ))
            .FirstOrDefaultAsync(cancellationToken);

        return result ?? new LoanSummary(0, 0, 0, 0);
    }

    public async Task<Loan> CreateAsync(Loan loan, CancellationToken cancellationToken = default)
    {
        context.Loans.Add(loan);
        await context.SaveChangesAsync(cancellationToken);
        return loan;
    }

    public async Task<Loan?> UpdateAsync(Loan loan, CancellationToken cancellationToken = default)
    {
        var existing = await context.Loans.FindAsync([loan.Id], cancellationToken);
        if (existing is null)
            return null;

        context.Entry(existing).CurrentValues.SetValues(loan);
        await context.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var loan = await context.Loans.FindAsync([id], cancellationToken);
        if (loan is null)
            return false;

        context.Loans.Remove(loan);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
