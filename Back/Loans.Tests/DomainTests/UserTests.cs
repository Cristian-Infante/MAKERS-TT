using Loans.Domain.Entities;
using Loans.Domain.Enums;
using Loans.Domain.Exceptions;

namespace Loans.Tests.DomainTests;

public class UserTests
{
    [Fact]
    public void Create_ValidData_ReturnsUser()
    {
        var user = User.Create("Test@Example.com", "hashedpwd", "Test User", UserRole.User);

        Assert.Equal("test@example.com", user.Email);
        Assert.Equal("hashedpwd", user.PasswordHash);
        Assert.Equal("Test User", user.FullName);
        Assert.Equal(UserRole.User, user.Role);
        Assert.False(user.IsDeleted);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyEmail_ThrowsDomainException(string email)
    {
        Assert.Throws<DomainException>(() => User.Create(email, "hash", "Full Name", UserRole.User));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyPasswordHash_ThrowsDomainException(string hash)
    {
        Assert.Throws<DomainException>(() => User.Create("a@b.com", hash, "Full Name", UserRole.User));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyFullName_ThrowsDomainException(string fullName)
    {
        Assert.Throws<DomainException>(() => User.Create("a@b.com", "hash", fullName, UserRole.User));
    }
}
