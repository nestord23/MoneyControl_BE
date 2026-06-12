using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MoneyControl.Models;
public class Loan
{
    public int Id { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Required]
    [MaxLength(150)]
    public string LenderName { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; }

    public LoanStatus Status { get; set; }
}