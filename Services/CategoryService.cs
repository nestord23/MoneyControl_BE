using MoneyControl.DTOs;
using MoneyControl.Models;
using MoneyControl.Repositories;

namespace MoneyControl.Services;

public class CategoryService(
    ICategoryRepository repository,
    IExpenseRepository expenseRepository) : ICategoryService
{
    public async Task<PagedResult<CategoryResponse>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await repository.GetAllAsync(page, pageSize, cancellationToken);
        return result.Map(MapToResponse);
    }

    public async Task<CategoryResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await repository.GetByIdAsync(id, cancellationToken);
        return category is null ? null : MapToResponse(category);
    }

    public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = new Category
        {
            Name = request.Name,
            Description = request.Description
        };

        var created = await repository.CreateAsync(category, cancellationToken);
        return MapToResponse(created);
    }

    public async Task<CategoryResponse?> UpdateAsync(int id, UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = new Category
        {
            Id = id,
            Name = request.Name,
            Description = request.Description
        };

        var updated = await repository.UpdateAsync(category, cancellationToken);
        return updated is null ? null : MapToResponse(updated);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var hasExpenses = await expenseRepository.ExistsByCategoryAsync(id, cancellationToken);
        if (hasExpenses)
            throw new InvalidOperationException("Cannot delete category with existing expenses. Remove or reassign the expenses first.");

        return await repository.DeleteAsync(id, cancellationToken);
    }

    private static CategoryResponse MapToResponse(Category category)
    {
        return new CategoryResponse(category.Id, category.Name, category.Description);
    }
}
