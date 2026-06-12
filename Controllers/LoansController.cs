using Microsoft.AspNetCore.Mvc;
using MoneyControl.DTOs;
using MoneyControl.Models;
using MoneyControl.Services;

namespace MoneyControl.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController(ILoanService loanService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<LoanResponse>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await loanService.GetAllAsync(page, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpGet("pending")]
    public async Task<ActionResult<PagedResult<LoanResponse>>> GetPending(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await loanService.GetPendingLoansAsync(page, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpGet("paid")]
    public async Task<ActionResult<PagedResult<LoanResponse>>> GetPaid(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await loanService.GetPaidLoansAsync(page, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LoanResponse>> GetById(int id, CancellationToken cancellationToken = default)
    {
        var loan = await loanService.GetByIdAsync(id, cancellationToken);
        if (loan is null)
            return NotFound();
        return Ok(loan);
    }

    [HttpGet("total/pending")]
    [ResponseCache(Duration = 30)]
    public async Task<ActionResult<decimal>> GetTotalPending(CancellationToken cancellationToken = default)
    {
        var total = await loanService.GetTotalPendingAmountAsync(cancellationToken);
        return Ok(total);
    }

    [HttpGet("total/paid")]
    [ResponseCache(Duration = 30)]
    public async Task<ActionResult<decimal>> GetTotalPaid(CancellationToken cancellationToken = default)
    {
        var total = await loanService.GetTotalPaidAmountAsync(cancellationToken);
        return Ok(total);
    }

    [HttpGet("summary")]
    [ResponseCache(Duration = 30)]
    public async Task<ActionResult<LoanSummaryResponse>> GetSummary(CancellationToken cancellationToken = default)
    {
        var summary = await loanService.GetSummaryAsync(cancellationToken);
        return Ok(summary);
    }

    [HttpPost]
    public async Task<ActionResult<LoanResponse>> Create([FromBody] CreateLoanRequest request, CancellationToken cancellationToken = default)
    {
        var loan = await loanService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = loan.Id }, loan);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<LoanResponse>> Update(int id, [FromBody] UpdateLoanRequest request, CancellationToken cancellationToken = default)
    {
        var loan = await loanService.UpdateAsync(id, request, cancellationToken);
        if (loan is null)
            return NotFound();
        return Ok(loan);
    }

    [HttpPatch("{id}/pay")]
    public async Task<ActionResult<LoanResponse>> MarkAsPaid(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var loan = await loanService.MarkAsPaidAsync(id, cancellationToken);
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
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await loanService.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return NotFound();
        return NoContent();
    }
}
