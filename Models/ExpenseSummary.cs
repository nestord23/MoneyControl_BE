namespace MoneyControl.Models;

public record ExpenseSummary(
    decimal TotalByDay,
    decimal TotalByWeek,
    decimal TotalByMonth,
    decimal TotalByYear,
    decimal TotalFixed,
    decimal TotalVariable
);
