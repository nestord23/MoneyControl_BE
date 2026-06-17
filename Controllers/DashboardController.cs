using Microsoft.AspNetCore.Mvc;
using MoneyControl.DTOs;
using MoneyControl.Services;

namespace MoneyControl.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController(IDashboardService dashboardService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<DashboardResponse>> GetDashboard(
        [FromQuery] int year,
        CancellationToken cancellationToken = default)
    {
        var data = await dashboardService.GetDashboardDataAsync(year, cancellationToken);
        return Ok(data);
    }
}
