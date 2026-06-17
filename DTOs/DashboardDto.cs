namespace MoneyControl.DTOs;

public record DashboardResponse(
    decimal Balance,
    decimal IncomesTotalYear,
    decimal ExpensesTotalYear,
    decimal IncomesTotalMonth,
    decimal ExpensesTotalMonth,
    decimal LoansTotalPending,
    decimal[] IncomesMonthlyTotals,
    decimal[] ExpensesMonthlyTotals,
    List<ExpenseResponse> RecentExpenses,
    List<IncomeResponse> RecentIncomes
);
