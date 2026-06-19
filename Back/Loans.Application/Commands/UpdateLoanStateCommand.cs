namespace Loans.Application.Commands;

public record UpdateLoanStateCommand(Guid LoanId, Guid RequestingUserId, string Status, string? RejectionReason);
