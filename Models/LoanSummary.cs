namespace MoneyControl.Models;

public record LoanSummary(
    decimal TotalPending,
    decimal TotalPaid,
    int PendingCount,
    int PaidCount
);
