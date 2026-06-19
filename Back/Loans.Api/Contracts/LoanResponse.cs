namespace Loans.Api.Contracts;

public sealed record LoanResponse(
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
