using MoneyControl.DTOs;

namespace MoneyControl.Services;

public interface ICategoryService
{
    Task<PagedResponse<CategoryResponse>> GetAllAsync(int page = 1, int pageSize = 20);
    Task<CategoryResponse?> GetByIdAsync(int id);
    Task<CategoryResponse> CreateAsync(CreateCategoryRequest request);
    Task<CategoryResponse?> UpdateAsync(int id, UpdateCategoryRequest request);
    Task<bool> DeleteAsync(int id);
}
