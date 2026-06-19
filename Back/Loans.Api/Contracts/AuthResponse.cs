namespace Loans.Api.Contracts;

public sealed record AuthResponse(
    string Token,
    string Email,
    string FullName,
    string Role);
