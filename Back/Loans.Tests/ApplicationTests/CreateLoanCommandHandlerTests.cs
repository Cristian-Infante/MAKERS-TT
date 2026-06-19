using Loans.Application.Caching;
using Loans.Application.Commands;
using Loans.Application.Exceptions;
using Loans.Application.Handlers;
using Loans.Application.Interfaces;
using Loans.Domain.Entities;
using Loans.Domain.Enums;
using Moq;

namespace Loans.Tests.ApplicationTests;

public class CreateLoanCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<ILoanRepository> _loanRepo = new();
    private readonly Mock<ICacheService> _cache = new();
    private readonly CreateLoanCommandHandler _handler;

    private static readonly Guid UserId = Guid.NewGuid();

    public CreateLoanCommandHandlerTests()
    {
        _uow.Setup(u => u.Users).Returns(_userRepo.Object);
        _uow.Setup(u => u.Loans).Returns(_loanRepo.Object);
        _handler = new CreateLoanCommandHandler(_uow.Object, _cache.Object);
    }

    [Fact]
    public async Task HandleAsync_UserExists_CreatesLoanAndInvalidatesCache()
    {
        var user = User.Create("user@test.com", "hash", "Test User", UserRole.User);
        _userRepo.Setup(r => r.GetByIdAsync(UserId)).ReturnsAsync(user);
        _loanRepo.Setup(r => r.AddAsync(It.IsAny<Loan>())).Returns(Task.CompletedTask);
        _uow.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _handler.HandleAsync(
            new CreateLoanCommand(UserId, 5000m, 24, "Business expansion"));

        Assert.Equal(UserId, result.UserId);
        Assert.Equal(5000m, result.Amount);
        Assert.Equal("Pending", result.Status);

        _cache.Verify(c => c.Remove(CacheKeys.UserLoans(UserId)), Times.Once);
        _cache.Verify(c => c.Remove(CacheKeys.AllLoans()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_UserNotFound_ThrowsNotFoundException()
    {
        _userRepo.Setup(r => r.GetByIdAsync(UserId)).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.HandleAsync(new CreateLoanCommand(UserId, 1000m, 12, "Test")));
    }
}
