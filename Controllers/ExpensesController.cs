using Microsoft.AspNetCore.Mvc;
using MoneyControl.DTOs;
using MoneyControl.Services;

namespace MoneyControl.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpensesController(IExpenseService expenseService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<ExpenseResponse>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await expenseService.GetAllAsync(page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ExpenseResponse>> GetById(int id)
    {
        var expense = await expenseService.GetByIdAsync(id);
        if (expense is null)
            return NotFound();
        return Ok(expense);
    }

    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<PagedResponse<ExpenseResponse>>> GetByCategory(
        int categoryId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await expenseService.GetByCategoryAsync(categoryId, page, pageSize);
        return Ok(result);
    }

    [HttpGet("total/day")]
    public async Task<ActionResult<decimal>> GetTotalByDay([FromQuery] DateTime date)
    {
        var total = await expenseService.GetTotalByDayAsync(date);
        return Ok(total);
    }

    [HttpGet("total/week")]
    public async Task<ActionResult<decimal>> GetTotalByWeek([FromQuery] DateTime date)
    {
        var total = await expenseService.GetTotalByWeekAsync(date);
        return Ok(total);
    }

    [HttpGet("total/month")]
    public async Task<ActionResult<decimal>> GetTotalByMonth(
        [FromQuery] int year,
        [FromQuery] int month)
    {
        var total = await expenseService.GetTotalByMonthAsync(year, month);
        return Ok(total);
    }

    [HttpGet("total/year")]
    public async Task<ActionResult<decimal>> GetTotalByYear([FromQuery] int year)
    {
        var total = await expenseService.GetTotalByYearAsync(year);
        return Ok(total);
    }

    [HttpGet("total/fixed")]
    public async Task<ActionResult<decimal>> GetTotalFixed()
    {
        var total = await expenseService.GetTotalFixedAsync();
        return Ok(total);
    }

    [HttpGet("total/variable")]
    public async Task<ActionResult<decimal>> GetTotalVariable()
    {
        var total = await expenseService.GetTotalVariableAsync();
        return Ok(total);
    }

    [HttpGet("summary")]
    public async Task<ActionResult<ExpenseSummaryResponse>> GetSummary()
    {
        var summary = await expenseService.GetSummaryAsync();
        return Ok(summary);
    }

    [HttpPost]
    public async Task<ActionResult<ExpenseResponse>> Create([FromBody] CreateExpenseRequest request)
    {
        var expense = await expenseService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = expense.Id }, expense);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ExpenseResponse>> Update(int id, [FromBody] UpdateExpenseRequest request)
    {
        var expense = await expenseService.UpdateAsync(id, request);
        if (expense is null)
            return NotFound();
        return Ok(expense);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var deleted = await expenseService.DeleteAsync(id);
        if (!deleted)
            return NotFound();
        return NoContent();
    }
}
