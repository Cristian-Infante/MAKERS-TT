using Loans.Application.Commands;
using Loans.Application.DTOs;
using Loans.Application.Exceptions;
using Loans.Application.Interfaces;
using Loans.Domain.Entities;
using Loans.Domain.Enums;

namespace Loans.Application.Handlers;

public class RegisterCommandHandler(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    ITokenService tokenService)
{
    public async Task<AuthResultDto> HandleAsync(RegisterCommand command)
    {
        if (await unitOfWork.Users.ExistsAsync(command.Email))
            throw new ConflictException($"Email '{command.Email}' is already registered.");

        var hash = passwordHasher.Hash(command.Password);
        var user = User.Create(command.Email, hash, command.FullName, UserRole.User);

        await unitOfWork.Users.AddAsync(user);
        await unitOfWork.CommitAsync();

        var userDto = new UserDto(user.Id, user.Email, user.FullName, user.Role.ToString());
        var token = tokenService.GenerateToken(userDto);

        return new AuthResultDto(token, userDto);
    }
}
