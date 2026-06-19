using Loans.Application.Caching;
using Loans.Application.Commands;
using Loans.Application.Exceptions;
using Loans.Application.Handlers;
using Loans.Application.Interfaces;
using Loans.Domain.Entities;
using Loans.Domain.Enums;
using Loans.Domain.Exceptions;
using Moq;

namespace Loans.Tests.ApplicationTests;

public class UpdateLoanStateCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<ILoanRepository> _loanRepo = new();
    private readonly Mock<ICacheService> _cache = new();
    private readonly UpdateLoanStateCommandHandler _handler;

    private static readonly Guid AdminId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid LoanId = Guid.NewGuid();

    public UpdateLoanStateCommandHandlerTests()
    {
        _uow.Setup(u => u.Users).Returns(_userRepo.Object);
        _uow.Setup(u => u.Loans).Returns(_loanRepo.Object);
        _uow.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _loanRepo.Setup(r => r.UpdateAsync(It.IsAny<Loan>())).Returns(Task.CompletedTask);
        _handler = new UpdateLoanStateCommandHandler(_uow.Object, _cache.Object);
    }

    private Loan PendingLoan() => Loan.Create(UserId, 1000m, 12, "Test purpose");

    [Fact]
    public async Task HandleAsync_PendingToApproved_SetsApprovedStatus()
    {
        var loan = PendingLoan();
        _loanRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(loan);
        _userRepo.Setup(r => r.GetByIdAsync(loan.UserId))
            .ReturnsAsync(User.Create("u@t.com", "h", "User Name", UserRole.User));

        var result = await _handler.HandleAsync(
            new UpdateLoanStateCommand(LoanId, AdminId, "Approved", null));

        Assert.Equal("Approved", result.Status);
    }

    [Fact]
    public async Task HandleAsync_PendingToRejectedWithReason_SetsRejectedStatus()
    {
        var loan = PendingLoan();
        _loanRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(loan);
        _userRepo.Setup(r => r.GetByIdAsync(loan.UserId))
            .ReturnsAsync(User.Create("u@t.com", "h", "User Name", UserRole.User));

        var result = await _handler.HandleAsync(
            new UpdateLoanStateCommand(LoanId, AdminId, "Rejected", "Low credit score"));

        Assert.Equal("Rejected", result.Status);
        Assert.Equal("Low credit score", result.RejectionReason);
    }

    [Fact]
    public async Task HandleAsync_LoanNotFound_ThrowsNotFoundException()
    {
        _loanRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Loan?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.HandleAsync(new UpdateLoanStateCommand(LoanId, AdminId, "Approved", null)));
    }

    [Fact]
    public async Task HandleAsync_ApproveNonPendingLoan_ThrowsInvalidLoanStateException()
    {
        var loan = PendingLoan();
        loan.Approve();
        _loanRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(loan);

        await Assert.ThrowsAsync<InvalidLoanStateException>(
            () => _handler.HandleAsync(new UpdateLoanStateCommand(LoanId, AdminId, "Approved", null)));
    }
}
