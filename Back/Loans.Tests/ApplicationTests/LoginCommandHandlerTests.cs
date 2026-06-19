using Loans.Application.Commands;
using Loans.Application.Exceptions;
using Loans.Application.Handlers;
using Loans.Application.Interfaces;
using Loans.Domain.Entities;
using Loans.Domain.Enums;
using Moq;

namespace Loans.Tests.ApplicationTests;

public class LoginCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IPasswordHasher> _hasher = new();
    private readonly Mock<ITokenService> _tokenService = new();
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _uow.Setup(u => u.Users).Returns(_userRepo.Object);
        _handler = new LoginCommandHandler(_uow.Object, _hasher.Object, _tokenService.Object);
    }

    [Fact]
    public async Task HandleAsync_ValidCredentials_ReturnsToken()
    {
        var user = User.Create("user@test.com", "hashedpwd", "Test User", UserRole.User);
        _userRepo.Setup(r => r.GetByEmailAsync("user@test.com")).ReturnsAsync(user);
        _hasher.Setup(h => h.Verify("123", "hashedpwd")).Returns(true);
        _tokenService.Setup(t => t.GenerateToken(It.IsAny<Application.DTOs.UserDto>())).Returns("token123");

        var result = await _handler.HandleAsync(new LoginCommand("user@test.com", "123"));

        Assert.Equal("token123", result.Token);
        Assert.Equal("user@test.com", result.User.Email);
    }

    [Fact]
    public async Task HandleAsync_UserNotFound_ThrowsInvalidCredentialsException()
    {
        _userRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<InvalidCredentialsException>(
            () => _handler.HandleAsync(new LoginCommand("missing@test.com", "123")));
    }

    [Fact]
    public async Task HandleAsync_WrongPassword_ThrowsInvalidCredentialsException()
    {
        var user = User.Create("user@test.com", "hashedpwd", "Test User", UserRole.User);
        _userRepo.Setup(r => r.GetByEmailAsync("user@test.com")).ReturnsAsync(user);
        _hasher.Setup(h => h.Verify("wrong", "hashedpwd")).Returns(false);

        await Assert.ThrowsAsync<InvalidCredentialsException>(
            () => _handler.HandleAsync(new LoginCommand("user@test.com", "wrong")));
    }
}
