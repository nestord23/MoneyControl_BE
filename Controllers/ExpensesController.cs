using Microsoft.AspNetCore.Mvc;
using MoneyControl.DTOs;
using MoneyControl.Models;
using MoneyControl.Services;

namespace MoneyControl.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpensesController(IExpenseService expenseService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<ExpenseResponse>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await expenseService.GetAllAsync(page, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ExpenseResponse>> GetById(int id, CancellationToken cancellationToken = default)
    {
        var expense = await expenseService.GetByIdAsync(id, cancellationToken);
        if (expense is null)
            return NotFound();
        return Ok(expense);
    }

    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<PagedResult<ExpenseResponse>>> GetByCategory(
        int categoryId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await expenseService.GetByCategoryAsync(categoryId, page, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpGet("total/day")]
    public async Task<ActionResult<decimal>> GetTotalByDay([FromQuery] DateTime date, CancellationToken cancellationToken = default)
    {
        var total = await expenseService.GetTotalByDayAsync(date, cancellationToken);
        return Ok(total);
    }

    [HttpGet("total/week")]
    public async Task<ActionResult<decimal>> GetTotalByWeek([FromQuery] DateTime date, CancellationToken cancellationToken = default)
    {
        var total = await expenseService.GetTotalByWeekAsync(date, cancellationToken);
        return Ok(total);
    }

    [HttpGet("total/month")]
    public async Task<ActionResult<decimal>> GetTotalByMonth(
        [FromQuery] int year,
        [FromQuery] int month,
        CancellationToken cancellationToken = default)
    {
        var total = await expenseService.GetTotalByMonthAsync(year, month, cancellationToken);
        return Ok(total);
    }

    [HttpGet("monthly")]
    public async Task<ActionResult<decimal[]>> GetMonthlyTotals([FromQuery] int year, CancellationToken cancellationToken = default)
    {
        var totals = await expenseService.GetMonthlyTotalsAsync(year, cancellationToken);
        return Ok(totals);
    }

    [HttpGet("total/year")]
    public async Task<ActionResult<decimal>> GetTotalByYear([FromQuery] int year, CancellationToken cancellationToken = default)
    {
        var total = await expenseService.GetTotalByYearAsync(year, cancellationToken);
        return Ok(total);
    }

    [HttpGet("total/fixed")]
    [ResponseCache(Duration = 30)]
    public async Task<ActionResult<decimal>> GetTotalFixed(CancellationToken cancellationToken = default)
    {
        var total = await expenseService.GetTotalFixedAsync(cancellationToken);
        return Ok(total);
    }

    [HttpGet("total/variable")]
    [ResponseCache(Duration = 30)]
    public async Task<ActionResult<decimal>> GetTotalVariable(CancellationToken cancellationToken = default)
    {
        var total = await expenseService.GetTotalVariableAsync(cancellationToken);
        return Ok(total);
    }

    [HttpGet("summary")]
    [ResponseCache(Duration = 30)]
    public async Task<ActionResult<ExpenseSummaryResponse>> GetSummary(CancellationToken cancellationToken = default)
    {
        var summary = await expenseService.GetSummaryAsync(cancellationToken);
        return Ok(summary);
    }

    [HttpPost]
    public async Task<ActionResult<ExpenseResponse>> Create([FromBody] CreateExpenseRequest request, CancellationToken cancellationToken = default)
    {
        var expense = await expenseService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = expense.Id }, expense);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ExpenseResponse>> Update(int id, [FromBody] UpdateExpenseRequest request, CancellationToken cancellationToken = default)
    {
        var expense = await expenseService.UpdateAsync(id, request, cancellationToken);
        if (expense is null)
            return NotFound();
        return Ok(expense);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await expenseService.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return NotFound();
        return NoContent();
    }
}
