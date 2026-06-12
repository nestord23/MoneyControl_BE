using System.ComponentModel.DataAnnotations;
using MoneyControl.Models;

namespace MoneyControl.DTOs;

public record CreateLoanRequest(
    [Required][Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
    decimal Amount,

    [Required][MaxLength(150)]
    string LenderName,

    [Required]
    DateTime Date
);

public record UpdateLoanRequest(
    [Required][Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
    decimal Amount,

    [Required][MaxLength(150)]
    string LenderName,

    [Required]
    DateTime Date,

    [Required]
    LoanStatus Status
);

public record LoanResponse(
    int Id,
    decimal Amount,
    string LenderName,
    DateTime Date,
    LoanStatus Status
);

public record LoanSummaryResponse(
    decimal TotalPending,
    decimal TotalPaid,
    int PendingCount,
    int PaidCount
);
