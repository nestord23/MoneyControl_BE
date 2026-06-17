using MoneyControl.DTOs;

namespace MoneyControl.Services;

public class DashboardService(
    IExpenseService expenseService,
    IIncomeService incomeService,
    ILoanService loanService) : IDashboardService
{
    public async Task<DashboardResponse> GetDashboardDataAsync(int year, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var month = now.Month;

        var incomesTotalYear = await incomeService.GetTotalByYearAsync(year, cancellationToken);
        var expensesTotalYear = await expenseService.GetTotalByYearAsync(year, cancellationToken);
        var incomesTotalMonth = await incomeService.GetTotalByMonthAsync(year, month, cancellationToken);
        var expensesTotalMonth = await expenseService.GetTotalByMonthAsync(year, month, cancellationToken);
        var loansTotalPending = await loanService.GetTotalPendingAmountAsync(cancellationToken);
        var incomesMonthly = await incomeService.GetMonthlyTotalsAsync(year, cancellationToken);
        var expensesMonthly = await expenseService.GetMonthlyTotalsAsync(year, cancellationToken);

        var recentExpenses = await expenseService.GetAllAsync(1, 5, cancellationToken);
        var recentIncomes = await incomeService.GetAllAsync(1, 5, cancellationToken);

        return new DashboardResponse(
            incomesTotalYear - expensesTotalYear,
            incomesTotalYear,
            expensesTotalYear,
            incomesTotalMonth,
            expensesTotalMonth,
            loansTotalPending,
            incomesMonthly,
            expensesMonthly,
            recentExpenses.Items.ToList(),
            recentIncomes.Items.ToList()
        );
    }
}
