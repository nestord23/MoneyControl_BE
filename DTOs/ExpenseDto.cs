using System.ComponentModel.DataAnnotations;
using MoneyControl.Models;

namespace MoneyControl.DTOs;

public record CreateExpenseRequest(
    [Required][Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
    decimal Amount,

    [Required]
    DateTime Date,

    [MaxLength(500)]
    string? Description,

    [Required]
    ExpenseType Type,

    [Required]
    int CategoryId
);

public record UpdateExpenseRequest(
    [Required][Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
    decimal Amount,

    [Required]
    DateTime Date,

    [MaxLength(500)]
    string? Description,

    [Required]
    ExpenseType Type,

    [Required]
    int CategoryId
);

public record ExpenseResponse(
    int Id,
    decimal Amount,
    DateTime Date,
    string? Description,
    ExpenseType Type,
    int CategoryId,
    string CategoryName
);

public record ExpenseSummaryResponse(
    decimal TotalByDay,
    decimal TotalByWeek,
    decimal TotalByMonth,
    decimal TotalByYear,
    decimal TotalFixed,
    decimal TotalVariable
);
