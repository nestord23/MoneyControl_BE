namespace MoneyControl.Models;

public record IncomeSummary(
    decimal TotalByDay,
    decimal TotalByWeek,
    decimal TotalByMonth,
    decimal TotalByYear
);
