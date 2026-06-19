using Loans.Domain.Enums;
using Loans.Domain.Exceptions;

namespace Loans.Domain.Entities;

public class Loan
{
    private Loan() { }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public decimal Amount { get; private set; }
    public int TermInMonths { get; private set; }
    public string Purpose { get; private set; } = string.Empty;
    public LoanStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public DateTime? RejectedAt { get; private set; }
    public string? RejectionReason { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    public User? User { get; private set; }

    public static Loan Reconstitute(
        Guid id, Guid userId, decimal amount, int termInMonths, string purpose,
        LoanStatus status, DateTime createdAt, DateTime? approvedAt, DateTime? rejectedAt,
        string? rejectionReason, bool isDeleted, DateTime? deletedAt)
    {
        return new Loan
        {
            Id = id,
            UserId = userId,
            Amount = amount,
            TermInMonths = termInMonths,
            Purpose = purpose,
            Status = status,
            CreatedAt = createdAt,
            ApprovedAt = approvedAt,
            RejectedAt = rejectedAt,
            RejectionReason = rejectionReason,
            IsDeleted = isDeleted,
            DeletedAt = deletedAt
        };
    }

    public static Loan Create(Guid userId, decimal amount, int termInMonths, string purpose)
    {
        if (amount <= 0)
            throw new DomainException("Amount must be greater than zero.");
        if (termInMonths < 1 || termInMonths > 120)
            throw new DomainException("Term must be between 1 and 120 months.");
        if (string.IsNullOrWhiteSpace(purpose))
            throw new DomainException("Purpose is required.");

        return new Loan
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Amount = amount,
            TermInMonths = termInMonths,
            Purpose = purpose.Trim(),
            Status = LoanStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
    }

    public void Approve()
    {
        if (Status != LoanStatus.Pending)
            throw new InvalidLoanStateException(Status.ToString(), nameof(Approve));

        Status = LoanStatus.Approved;
        ApprovedAt = DateTime.UtcNow;
    }

    public void Reject(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Rejection reason is required.");
        if (Status != LoanStatus.Pending)
            throw new InvalidLoanStateException(Status.ToString(), nameof(Reject));

        Status = LoanStatus.Rejected;
        RejectedAt = DateTime.UtcNow;
        RejectionReason = reason.Trim();
    }
}
