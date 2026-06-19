using System.ComponentModel.DataAnnotations;

namespace Loans.Api.Contracts;

public sealed class RegisterRequest
{
    [Required]
    [EmailAddress]
    public required string Email { get; init; }

    [Required]
    [MinLength(3)]
    public required string Password { get; init; }

    [Required]
    [StringLength(256)]
    public required string FullName { get; init; }
}
