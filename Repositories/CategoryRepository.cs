using MoneyControl.DTOs;

namespace MoneyControl.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly List<CategoryResponse> _categories = [];
    private int _nextId = 1;

    public Task<IEnumerable<CategoryResponse>> GetAllAsync()
    {
        return Task.FromResult(_categories.AsEnumerable());
    }

    public Task<CategoryResponse?> GetByIdAsync(int id)
    {
        var category = _categories.FirstOrDefault(c => c.Id == id);
        return Task.FromResult(category);
    }

    public Task<CategoryResponse> CreateAsync(CreateCategoryRequest request)
    {
        var category = new CategoryResponse(_nextId++, request.Name, request.Description, request.Type, DateTime.UtcNow);
        _categories.Add(category);
        return Task.FromResult(category);
    }

    public Task<CategoryResponse?> UpdateAsync(int id, UpdateCategoryRequest request)
    {
        var index = _categories.FindIndex(c => c.Id == id);
        if (index == -1)
            return Task.FromResult<CategoryResponse?>(null);

        var updated = _categories[index] with
        {
            // record with-expressions on properties not defined as init-only
        };

        _categories[index] = new CategoryResponse(id, request.Name, request.Description, request.Type, _categories[index].CreatedAt);
        return Task.FromResult<CategoryResponse?>(_categories[index]);
    }

    public Task<bool> DeleteAsync(int id)
    {
        var removed = _categories.RemoveAll(c => c.Id == id);
        return Task.FromResult(removed > 0);
    }
}
