using System.ComponentModel.DataAnnotations;

namespace Loans.Api.Contracts;

public sealed class CreateLoanRequest
{
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
    public required decimal Amount { get; init; }

    [Required]
    [Range(1, 120, ErrorMessage = "TermInMonths must be between 1 and 120.")]
    public required int TermInMonths { get; init; }

    [Required]
    [StringLength(500)]
    public required string Purpose { get; init; }
}
