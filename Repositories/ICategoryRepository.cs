using MoneyControl.Models;

namespace MoneyControl.Repositories;

public interface ICategoryRepository
{
    Task<PagedResult<Category>> GetAllAsync(int page = 1, int pageSize = 20);
    Task<Category?> GetByIdAsync(int id);
    Task<Category> CreateAsync(Category category);
    Task<Category?> UpdateAsync(Category category);
    Task<bool> DeleteAsync(int id);
}
