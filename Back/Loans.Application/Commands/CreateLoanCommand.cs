namespace Loans.Application.Commands;

public record CreateLoanCommand(Guid UserId, decimal Amount, int TermInMonths, string Purpose);
