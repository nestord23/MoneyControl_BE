using System.ComponentModel.DataAnnotations;

namespace MoneyControl.DTOs;

public record CreateIncomeRequest(
    [Required][Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
    decimal Amount,

    [Required]
    DateTime Date,

    [MaxLength(500)]
    string? Description
);

public record UpdateIncomeRequest(
    [Required][Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
    decimal Amount,

    [Required]
    DateTime Date,

    [MaxLength(500)]
    string? Description
);

public record IncomeResponse(
    int Id,
    decimal Amount,
    DateTime Date,
    string? Description
);

public record IncomeSummaryResponse(
    decimal TotalByDay,
    decimal TotalByWeek,
    decimal TotalByMonth,
    decimal TotalByYear
);
