using Microsoft.AspNetCore.Mvc;
using MoneyControl.DTOs;
using MoneyControl.Services;

namespace MoneyControl.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<CategoryResponse>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var categories = await categoryService.GetAllAsync(page, pageSize);
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryResponse>> GetById(int id)
    {
        var category = await categoryService.GetByIdAsync(id);
        if (category is null)
            return NotFound();
        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryResponse>> Create([FromBody] CreateCategoryRequest request)
    {
        var category = await categoryService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CategoryResponse>> Update(int id, [FromBody] UpdateCategoryRequest request)
    {
        var category = await categoryService.UpdateAsync(id, request);
        if (category is null)
            return NotFound();
        return Ok(category);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var deleted = await categoryService.DeleteAsync(id);
        if (!deleted)
            return NotFound();
        return NoContent();
    }
}
