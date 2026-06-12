using Microsoft.EntityFrameworkCore;
using MoneyControl.Data;
using MoneyControl.Models;

namespace MoneyControl.Repositories;

public class CategoryRepository(AppDbContext context) : ICategoryRepository
{
    public async Task<PagedResult<Category>> GetAllAsync(int page = 1, int pageSize = 20)
    {
        var query = context.Categories.AsNoTracking();
        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Category>(items, totalCount, page, pageSize, totalPages);
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await context.Categories.FindAsync(id);
    }

    public async Task<Category> CreateAsync(Category category)
    {
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        return category;
    }

    public async Task<Category?> UpdateAsync(Category category)
    {
        var existing = await context.Categories.FindAsync(category.Id);
        if (existing is null)
            return null;

        context.Entry(existing).CurrentValues.SetValues(category);
        await context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await context.Categories.FindAsync(id);
        if (category is null)
            return false;

        context.Categories.Remove(category);
        await context.SaveChangesAsync();
        return true;
    }
}
