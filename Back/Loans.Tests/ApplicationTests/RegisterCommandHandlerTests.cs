using Loans.Application.Commands;
using Loans.Application.Exceptions;
using Loans.Application.Handlers;
using Loans.Application.Interfaces;
using Loans.Domain.Entities;
using Loans.Domain.Enums;
using Moq;

namespace Loans.Tests.ApplicationTests;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IPasswordHasher> _hasher = new();
    private readonly Mock<ITokenService> _tokenService = new();
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _uow.Setup(u => u.Users).Returns(_userRepo.Object);
        _handler = new RegisterCommandHandler(_uow.Object, _hasher.Object, _tokenService.Object);
    }

    [Fact]
    public async Task HandleAsync_NewEmail_ReturnsToken()
    {
        _userRepo.Setup(r => r.ExistsAsync("new@test.com")).ReturnsAsync(false);
        _hasher.Setup(h => h.Hash("123")).Returns("hashedpwd");
        _userRepo.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
        _uow.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _tokenService.Setup(t => t.GenerateToken(It.IsAny<Application.DTOs.UserDto>())).Returns("token456");

        var result = await _handler.HandleAsync(new RegisterCommand("new@test.com", "123", "New User"));

        Assert.Equal("token456", result.Token);
        Assert.Equal("new@test.com", result.User.Email);
    }

    [Fact]
    public async Task HandleAsync_DuplicateEmail_ThrowsConflictException()
    {
        _userRepo.Setup(r => r.ExistsAsync("existing@test.com")).ReturnsAsync(true);

        await Assert.ThrowsAsync<ConflictException>(
            () => _handler.HandleAsync(new RegisterCommand("existing@test.com", "123", "User")));
    }
}
