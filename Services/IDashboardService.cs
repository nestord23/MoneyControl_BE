using MoneyControl.DTOs;

namespace MoneyControl.Services;

public interface IDashboardService
{
    Task<DashboardResponse> GetDashboardDataAsync(int year, CancellationToken cancellationToken = default);
}
