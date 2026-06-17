using Microsoft.AspNetCore.Mvc;
using MoneyControl.DTOs;
using MoneyControl.Models;
using MoneyControl.Services;

namespace MoneyControl.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IncomesController(IIncomeService incomeService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<IncomeResponse>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await incomeService.GetAllAsync(page, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IncomeResponse>> GetById(int id, CancellationToken cancellationToken = default)
    {
        var income = await incomeService.GetByIdAsync(id, cancellationToken);
        if (income is null)
            return NotFound();
        return Ok(income);
    }

    [HttpGet("total/day")]
    public async Task<ActionResult<decimal>> GetTotalByDay([FromQuery] DateTime date, CancellationToken cancellationToken = default)
    {
        var total = await incomeService.GetTotalByDayAsync(date, cancellationToken);
        return Ok(total);
    }

    [HttpGet("total/week")]
    public async Task<ActionResult<decimal>> GetTotalByWeek([FromQuery] DateTime date, CancellationToken cancellationToken = default)
    {
        var total = await incomeService.GetTotalByWeekAsync(date, cancellationToken);
        return Ok(total);
    }

    [HttpGet("total/month")]
    public async Task<ActionResult<decimal>> GetTotalByMonth(
        [FromQuery] int year,
        [FromQuery] int month,
        CancellationToken cancellationToken = default)
    {
        var total = await incomeService.GetTotalByMonthAsync(year, month, cancellationToken);
        return Ok(total);
    }

    [HttpGet("monthly")]
    public async Task<ActionResult<decimal[]>> GetMonthlyTotals([FromQuery] int year, CancellationToken cancellationToken = default)
    {
        var totals = await incomeService.GetMonthlyTotalsAsync(year, cancellationToken);
        return Ok(totals);
    }

    [HttpGet("total/year")]
    public async Task<ActionResult<decimal>> GetTotalByYear([FromQuery] int year, CancellationToken cancellationToken = default)
    {
        var total = await incomeService.GetTotalByYearAsync(year, cancellationToken);
        return Ok(total);
    }

    [HttpGet("summary")]
    [ResponseCache(Duration = 30)]
    public async Task<ActionResult<IncomeSummaryResponse>> GetSummary(CancellationToken cancellationToken = default)
    {
        var summary = await incomeService.GetSummaryAsync(cancellationToken);
        return Ok(summary);
    }

    [HttpPost]
    public async Task<ActionResult<IncomeResponse>> Create([FromBody] CreateIncomeRequest request, CancellationToken cancellationToken = default)
    {
        var income = await incomeService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = income.Id }, income);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<IncomeResponse>> Update(int id, [FromBody] UpdateIncomeRequest request, CancellationToken cancellationToken = default)
    {
        var income = await incomeService.UpdateAsync(id, request, cancellationToken);
        if (income is null)
            return NotFound();
        return Ok(income);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await incomeService.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return NotFound();
        return NoContent();
    }
}
