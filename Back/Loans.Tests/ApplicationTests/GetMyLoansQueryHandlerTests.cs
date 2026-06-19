using Loans.Application.Caching;
using Loans.Application.DTOs;
using Loans.Application.Handlers;
using Loans.Application.Interfaces;
using Loans.Application.Queries;
using Moq;

namespace Loans.Tests.ApplicationTests;

public class GetMyLoansQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<ILoanRepository> _loanRepo = new();
    private readonly Mock<ICacheService> _cache = new();
    private readonly GetMyLoansQueryHandler _handler;

    private static readonly Guid UserId = Guid.NewGuid();

    private static readonly List<LoanDto> SampleLoans =
    [
        new LoanDto(Guid.NewGuid(), UserId, "User", 1000m, 12, "Test", "Pending",
            DateTime.UtcNow, null, null, null)
    ];

    public GetMyLoansQueryHandlerTests()
    {
        _uow.Setup(u => u.Loans).Returns(_loanRepo.Object);
        _handler = new GetMyLoansQueryHandler(_uow.Object, _cache.Object);
    }

    [Fact]
    public async Task HandleAsync_CacheHit_DoesNotCallRepository()
    {
        _cache.Setup(c => c.Get<List<LoanDto>>(CacheKeys.UserLoans(UserId))).Returns(SampleLoans);

        var result = await _handler.HandleAsync(new GetMyLoansQuery(UserId));

        Assert.Single(result);
        _loanRepo.Verify(r => r.GetByUserIdAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_CacheMiss_CallsRepositoryAndCaches()
    {
        _cache.Setup(c => c.Get<List<LoanDto>>(CacheKeys.UserLoans(UserId))).Returns((List<LoanDto>?)null);
        _loanRepo.Setup(r => r.GetByUserIdAsync(UserId)).ReturnsAsync(SampleLoans);

        var result = await _handler.HandleAsync(new GetMyLoansQuery(UserId));

        Assert.Single(result);
        _loanRepo.Verify(r => r.GetByUserIdAsync(UserId), Times.Once);
        _cache.Verify(c => c.Set(CacheKeys.UserLoans(UserId), It.IsAny<List<LoanDto>>(), null), Times.Once);
    }
}
