using Microsoft.AspNetCore.Mvc;
using MoneyControl.DTOs;
using MoneyControl.Services;

namespace MoneyControl.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IncomesController(IIncomeService incomeService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<IncomeResponse>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await incomeService.GetAllAsync(page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IncomeResponse>> GetById(int id)
    {
        var income = await incomeService.GetByIdAsync(id);
        if (income is null)
            return NotFound();
        return Ok(income);
    }

    [HttpGet("total/day")]
    public async Task<ActionResult<decimal>> GetTotalByDay([FromQuery] DateTime date)
    {
        var total = await incomeService.GetTotalByDayAsync(date);
        return Ok(total);
    }

    [HttpGet("total/week")]
    public async Task<ActionResult<decimal>> GetTotalByWeek([FromQuery] DateTime date)
    {
        var total = await incomeService.GetTotalByWeekAsync(date);
        return Ok(total);
    }

    [HttpGet("total/month")]
    public async Task<ActionResult<decimal>> GetTotalByMonth(
        [FromQuery] int year,
        [FromQuery] int month)
    {
        var total = await incomeService.GetTotalByMonthAsync(year, month);
        return Ok(total);
    }

    [HttpGet("total/year")]
    public async Task<ActionResult<decimal>> GetTotalByYear([FromQuery] int year)
    {
        var total = await incomeService.GetTotalByYearAsync(year);
        return Ok(total);
    }

    [HttpGet("summary")]
    public async Task<ActionResult<IncomeSummaryResponse>> GetSummary()
    {
        var summary = await incomeService.GetSummaryAsync();
        return Ok(summary);
    }

    [HttpPost]
    public async Task<ActionResult<IncomeResponse>> Create([FromBody] CreateIncomeRequest request)
    {
        var income = await incomeService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = income.Id }, income);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<IncomeResponse>> Update(int id, [FromBody] UpdateIncomeRequest request)
    {
        var income = await incomeService.UpdateAsync(id, request);
        if (income is null)
            return NotFound();
        return Ok(income);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var deleted = await incomeService.DeleteAsync(id);
        if (!deleted)
            return NotFound();
        return NoContent();
    }
}
