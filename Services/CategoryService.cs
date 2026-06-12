using MoneyControl.DTOs;
using MoneyControl.Models;
using MoneyControl.Repositories;

namespace MoneyControl.Services;

public class CategoryService(ICategoryRepository repository) : ICategoryService
{
    public async Task<PagedResponse<CategoryResponse>> GetAllAsync(int page = 1, int pageSize = 20)
    {
        var result = await repository.GetAllAsync(page, pageSize);
        return new PagedResponse<CategoryResponse>(
            result.Items.Select(MapToResponse),
            result.TotalCount,
            result.Page,
            result.PageSize,
            result.TotalPages
        );
    }

    public async Task<CategoryResponse?> GetByIdAsync(int id)
    {
        var category = await repository.GetByIdAsync(id);
        return category is null ? null : MapToResponse(category);
    }

    public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request)
    {
        var category = new Category
        {
            Name = request.Name,
            Description = request.Description
        };

        var created = await repository.CreateAsync(category);
        return MapToResponse(created);
    }

    public async Task<CategoryResponse?> UpdateAsync(int id, UpdateCategoryRequest request)
    {
        var category = new Category
        {
            Id = id,
            Name = request.Name,
            Description = request.Description
        };

        var updated = await repository.UpdateAsync(category);
        return updated is null ? null : MapToResponse(updated);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await repository.DeleteAsync(id);
    }

    private static CategoryResponse MapToResponse(Category category)
    {
        return new CategoryResponse(category.Id, category.Name, category.Description);
    }
}
