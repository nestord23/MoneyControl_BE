using MoneyControl.DTOs;
using MoneyControl.Repositories;

namespace MoneyControl.Services;

public class CategoryService(ICategoryRepository repository) : ICategoryService
{
    public Task<IEnumerable<CategoryResponse>> GetAllAsync()
    {
        return repository.GetAllAsync();
    }

    public Task<CategoryResponse?> GetByIdAsync(int id)
    {
        return repository.GetByIdAsync(id);
    }

    public Task<CategoryResponse> CreateAsync(CreateCategoryRequest request)
    {
        return repository.CreateAsync(request);
    }

    public Task<CategoryResponse?> UpdateAsync(int id, UpdateCategoryRequest request)
    {
        return repository.UpdateAsync(id, request);
    }

    public Task<bool> DeleteAsync(int id)
    {
        return repository.DeleteAsync(id);
    }
}
