namespace Loans.Application.Commands;

public record RegisterCommand(string Email, string Password, string FullName);
