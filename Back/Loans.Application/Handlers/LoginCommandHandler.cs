using Loans.Application.Commands;
using Loans.Application.DTOs;
using Loans.Application.Exceptions;
using Loans.Application.Interfaces;

namespace Loans.Application.Handlers;

public class LoginCommandHandler(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    ITokenService tokenService)
{
    public async Task<AuthResultDto> HandleAsync(LoginCommand command)
    {
        var user = await unitOfWork.Users.GetByEmailAsync(command.Email);
        if (user is null)
            throw new InvalidCredentialsException();

        if (!passwordHasher.Verify(command.Password, user.PasswordHash))
            throw new InvalidCredentialsException();

        var userDto = new UserDto(user.Id, user.Email, user.FullName, user.Role.ToString());
        var token = tokenService.GenerateToken(userDto);

        return new AuthResultDto(token, userDto);
    }
}
