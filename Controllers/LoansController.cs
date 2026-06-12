using Microsoft.AspNetCore.Mvc;
using MoneyControl.DTOs;
using MoneyControl.Services;

namespace MoneyControl.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController(ILoanService loanService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<LoanResponse>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await loanService.GetAllAsync(page, pageSize);
        return Ok(result);
    }

    [HttpGet("pending")]
    public async Task<ActionResult<PagedResponse<LoanResponse>>> GetPending(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await loanService.GetPendingLoansAsync(page, pageSize);
        return Ok(result);
    }

    [HttpGet("paid")]
    public async Task<ActionResult<PagedResponse<LoanResponse>>> GetPaid(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await loanService.GetPaidLoansAsync(page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LoanResponse>> GetById(int id)
    {
        var loan = await loanService.GetByIdAsync(id);
        if (loan is null)
            return NotFound();
        return Ok(loan);
    }

    [HttpGet("total/pending")]
    public async Task<ActionResult<decimal>> GetTotalPending()
    {
        var total = await loanService.GetTotalPendingAmountAsync();
        return Ok(total);
    }

    [HttpGet("total/paid")]
    public async Task<ActionResult<decimal>> GetTotalPaid()
    {
        var total = await loanService.GetTotalPaidAmountAsync();
        return Ok(total);
    }

    [HttpPost]
    public async Task<ActionResult<LoanResponse>> Create([FromBody] CreateLoanRequest request)
    {
        var loan = await loanService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = loan.Id }, loan);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<LoanResponse>> Update(int id, [FromBody] UpdateLoanRequest request)
    {
        var loan = await loanService.UpdateAsync(id, request);
        if (loan is null)
            return NotFound();
        return Ok(loan);
    }

    [HttpPatch("{id}/pay")]
    public async Task<ActionResult<LoanResponse>> MarkAsPaid(int id)
    {
        try
        {
            var loan = await loanService.MarkAsPaidAsync(id);
            if (loan is null)
                return NotFound();
            return Ok(loan);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var deleted = await loanService.DeleteAsync(id);
        if (!deleted)
            return NotFound();
        return NoContent();
    }
}
