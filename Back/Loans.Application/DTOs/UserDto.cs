namespace Loans.Application.DTOs;

public record UserDto(Guid Id, string Email, string FullName, string Role);
