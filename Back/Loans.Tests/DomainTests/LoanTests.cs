using Loans.Domain.Entities;
using Loans.Domain.Exceptions;

namespace Loans.Tests.DomainTests;

public class LoanTests
{
    private static readonly Guid UserId = Guid.NewGuid();

    [Fact]
    public void Create_ValidData_ReturnsLoan()
    {
        var loan = Loan.Create(UserId, 1000m, 12, "Home improvement");

        Assert.Equal(UserId, loan.UserId);
        Assert.Equal(1000m, loan.Amount);
        Assert.Equal(12, loan.TermInMonths);
        Assert.Equal("Home improvement", loan.Purpose);
        Assert.Equal(Domain.Enums.LoanStatus.Pending, loan.Status);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Create_InvalidAmount_ThrowsDomainException(decimal amount)
    {
        Assert.Throws<DomainException>(() => Loan.Create(UserId, amount, 12, "Purpose"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(121)]
    [InlineData(-5)]
    public void Create_InvalidTerm_ThrowsDomainException(int term)
    {
        Assert.Throws<DomainException>(() => Loan.Create(UserId, 1000m, term, "Purpose"));
    }

    [Fact]
    public void Create_EmptyPurpose_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() => Loan.Create(UserId, 1000m, 12, "   "));
    }

    [Fact]
    public void Approve_FromPending_SetsApproved()
    {
        var loan = Loan.Create(UserId, 500m, 6, "Car repair");

        loan.Approve();

        Assert.Equal(Domain.Enums.LoanStatus.Approved, loan.Status);
        Assert.NotNull(loan.ApprovedAt);
    }

    [Fact]
    public void Approve_FromApproved_ThrowsInvalidLoanStateException()
    {
        var loan = Loan.Create(UserId, 500m, 6, "Car repair");
        loan.Approve();

        Assert.Throws<InvalidLoanStateException>(() => loan.Approve());
    }

    [Fact]
    public void Approve_FromRejected_ThrowsInvalidLoanStateException()
    {
        var loan = Loan.Create(UserId, 500m, 6, "Car repair");
        loan.Reject("Not eligible");

        Assert.Throws<InvalidLoanStateException>(() => loan.Approve());
    }

    [Fact]
    public void Reject_FromPending_WithReason_SetsRejected()
    {
        var loan = Loan.Create(UserId, 500m, 6, "Car repair");

        loan.Reject("Insufficient income");

        Assert.Equal(Domain.Enums.LoanStatus.Rejected, loan.Status);
        Assert.NotNull(loan.RejectedAt);
        Assert.Equal("Insufficient income", loan.RejectionReason);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Reject_EmptyReason_ThrowsDomainException(string reason)
    {
        var loan = Loan.Create(UserId, 500m, 6, "Car repair");

        Assert.Throws<DomainException>(() => loan.Reject(reason));
    }

    [Fact]
    public void Reject_NotPending_ThrowsInvalidLoanStateException()
    {
        var loan = Loan.Create(UserId, 500m, 6, "Car repair");
        loan.Approve();

        Assert.Throws<InvalidLoanStateException>(() => loan.Reject("Some reason"));
    }
}
