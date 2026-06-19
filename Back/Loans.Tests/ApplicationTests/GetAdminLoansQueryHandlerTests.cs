using Loans.Application.Caching;
using Loans.Application.DTOs;
using Loans.Application.Handlers;
using Loans.Application.Interfaces;
using Loans.Application.Queries;
using Moq;

namespace Loans.Tests.ApplicationTests;

public class GetAdminLoansQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<ILoanRepository> _loanRepo = new();
    private readonly Mock<ICacheService> _cache = new();
    private readonly GetAdminLoansQueryHandler _handler;

    private static readonly List<LoanDto> SampleLoans =
    [
        new LoanDto(Guid.NewGuid(), Guid.NewGuid(), "User A", 2000m, 6, "Travel", "Pending",
            DateTime.UtcNow, null, null, null),
        new LoanDto(Guid.NewGuid(), Guid.NewGuid(), "User B", 5000m, 24, "Business", "Approved",
            DateTime.UtcNow, DateTime.UtcNow, null, null)
    ];

    public GetAdminLoansQueryHandlerTests()
    {
        _uow.Setup(u => u.Loans).Returns(_loanRepo.Object);
        _handler = new GetAdminLoansQueryHandler(_uow.Object, _cache.Object);
    }

    [Fact]
    public async Task HandleAsync_CacheHit_DoesNotCallRepository()
    {
        _cache.Setup(c => c.Get<List<LoanDto>>(CacheKeys.AllLoans())).Returns(SampleLoans);

        var result = await _handler.HandleAsync(new GetAdminLoansQuery());

        Assert.Equal(2, result.Count());
        _loanRepo.Verify(r => r.GetAllWithUserAsync(), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_CacheMiss_CallsRepositoryAndCaches()
    {
        _cache.Setup(c => c.Get<List<LoanDto>>(CacheKeys.AllLoans())).Returns((List<LoanDto>?)null);
        _loanRepo.Setup(r => r.GetAllWithUserAsync()).ReturnsAsync(SampleLoans);

        var result = await _handler.HandleAsync(new GetAdminLoansQuery());

        Assert.Equal(2, result.Count());
        _loanRepo.Verify(r => r.GetAllWithUserAsync(), Times.Once);
        _cache.Verify(c => c.Set(CacheKeys.AllLoans(), It.IsAny<List<LoanDto>>(), null), Times.Once);
    }
}
