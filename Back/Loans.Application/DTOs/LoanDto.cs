namespace Loans.Application.DTOs;

public record LoanDto(
    Guid Id,
    Guid UserId,
    string UserName,
    decimal Amount,
    int TermInMonths,
    string Purpose,
    string Status,
    DateTime CreatedAt,
    DateTime? ApprovedAt,
    DateTime? RejectedAt,
    string? RejectionReason);
