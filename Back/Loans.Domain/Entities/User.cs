using Loans.Domain.Enums;
using Loans.Domain.Exceptions;

namespace Loans.Domain.Entities;

public class User
{
    private readonly List<Loan> _loans = [];

    private User() { }

    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    public IReadOnlyCollection<Loan> Loans => _loans.AsReadOnly();

    public static User Reconstitute(
        Guid id, string email, string passwordHash, string fullName,
        UserRole role, DateTime createdAt, bool isDeleted, DateTime? deletedAt)
    {
        return new User
        {
            Id = id,
            Email = email,
            PasswordHash = passwordHash,
            FullName = fullName,
            Role = role,
            CreatedAt = createdAt,
            IsDeleted = isDeleted,
            DeletedAt = deletedAt
        };
    }

    public static User Create(string email, string passwordHash, string fullName, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email is required.");
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("Password hash is required.");
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("Full name is required.");

        return new User
        {
            Id = Guid.NewGuid(),
            Email = email.Trim().ToLowerInvariant(),
            PasswordHash = passwordHash,
            FullName = fullName.Trim(),
            Role = role,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
    }
}
