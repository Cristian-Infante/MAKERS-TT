namespace Loans.Domain.Exceptions;

public class InvalidLoanStateException(string currentState, string attemptedAction)
    : DomainException($"Cannot perform '{attemptedAction}' on a loan with status '{currentState}'.");
