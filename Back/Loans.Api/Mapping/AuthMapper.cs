using Loans.Api.Contracts;
using Loans.Application.Commands;
using Loans.Application.DTOs;

namespace Loans.Api.Mapping;

public static class AuthMapper
{
    public static LoginCommand ToCommand(this LoginRequest request) =>
        new(request.Email, request.Password);

    public static RegisterCommand ToCommand(this RegisterRequest request) =>
        new(request.Email, request.Password, request.FullName);

    public static AuthResponse ToResponse(this AuthResultDto dto) =>
        new(dto.Token, dto.User.Email, dto.User.FullName, dto.User.Role);
}
