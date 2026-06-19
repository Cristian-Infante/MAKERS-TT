using Dapper;
using Loans.Application.DTOs;
using Loans.Application.Interfaces;
using Loans.Domain.Entities;
using Loans.Domain.Enums;
using Loans.Infrastructure.Data;

namespace Loans.Infrastructure.Repositories;

public class LoanRepository(UnitOfWork unitOfWork) : ILoanRepository
{
    public async Task<IEnumerable<LoanDto>> GetByUserIdAsync(Guid userId)
    {
        const string sql = """
            SELECT l.Id, l.UserId, u.FullName AS UserName, l.Amount, l.TermInMonths,
                   l.Purpose, l.Status, l.CreatedAt, l.ApprovedAt, l.RejectedAt, l.RejectionReason
            FROM Loans l
            INNER JOIN Users u ON l.UserId = u.Id
            WHERE l.UserId = @UserId AND l.IsDeleted = 0
            ORDER BY l.CreatedAt DESC
            """;
        var rows = await unitOfWork.Connection.QueryAsync<LoanRow>(
            sql, new { UserId = userId }, unitOfWork.Transaction);
        return rows.Select(r => r.ToDto());
    }

    public async Task<IEnumerable<LoanDto>> GetAllWithUserAsync()
    {
        const string sql = """
            SELECT l.Id, l.UserId, u.FullName AS UserName, l.Amount, l.TermInMonths,
                   l.Purpose, l.Status, l.CreatedAt, l.ApprovedAt, l.RejectedAt, l.RejectionReason
            FROM Loans l
            INNER JOIN Users u ON l.UserId = u.Id
            WHERE l.IsDeleted = 0
            ORDER BY l.CreatedAt DESC
            """;
        var rows = await unitOfWork.Connection.QueryAsync<LoanRow>(
            sql, transaction: unitOfWork.Transaction);
        return rows.Select(r => r.ToDto());
    }

    public async Task<Loan?> GetByIdAsync(Guid id)
    {
        const string sql = """
            SELECT Id, UserId, Amount, TermInMonths, Purpose, Status,
                   CreatedAt, ApprovedAt, RejectedAt, RejectionReason, IsDeleted, DeletedAt
            FROM Loans
            WHERE Id = @Id AND IsDeleted = 0
            """;
        var row = await unitOfWork.Connection.QueryFirstOrDefaultAsync<LoanEntityRow>(
            sql, new { Id = id }, unitOfWork.Transaction);
        return row?.ToLoan();
    }

    public async Task AddAsync(Loan loan)
    {
        const string sql = """
            INSERT INTO Loans (Id, UserId, Amount, TermInMonths, Purpose, Status, CreatedAt, IsDeleted)
            VALUES (@Id, @UserId, @Amount, @TermInMonths, @Purpose, @Status, @CreatedAt, @IsDeleted)
            """;
        await unitOfWork.Connection.ExecuteAsync(sql, new
        {
            loan.Id,
            loan.UserId,
            loan.Amount,
            loan.TermInMonths,
            loan.Purpose,
            Status = loan.Status.ToString(),
            loan.CreatedAt,
            loan.IsDeleted
        }, unitOfWork.Transaction);
    }

    public async Task UpdateAsync(Loan loan)
    {
        const string sql = """
            UPDATE Loans
            SET Status          = @Status,
                ApprovedAt      = @ApprovedAt,
                RejectedAt      = @RejectedAt,
                RejectionReason = @RejectionReason
            WHERE Id = @Id
            """;
        await unitOfWork.Connection.ExecuteAsync(sql, new
        {
            loan.Id,
            Status = loan.Status.ToString(),
            loan.ApprovedAt,
            loan.RejectedAt,
            loan.RejectionReason
        }, unitOfWork.Transaction);
    }

    // DTO row — for JOIN queries (GetByUserId, GetAllWithUser)
    private sealed class LoanRow
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int TermInMonths { get; set; }
        public string Purpose { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime? RejectedAt { get; set; }
        public string? RejectionReason { get; set; }

        public LoanDto ToDto() => new(
            Id, UserId, UserName, Amount, TermInMonths, Purpose,
            Status, CreatedAt, ApprovedAt, RejectedAt, RejectionReason);
    }

    // Entity row — for GetByIdAsync (needs Loan entity for Approve/Reject)
    private sealed class LoanEntityRow
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public int TermInMonths { get; set; }
        public string Purpose { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime? RejectedAt { get; set; }
        public string? RejectionReason { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public Loan ToLoan() => Loan.Reconstitute(
            Id, UserId, Amount, TermInMonths, Purpose,
            Enum.Parse<LoanStatus>(Status), CreatedAt, ApprovedAt,
            RejectedAt, RejectionReason, IsDeleted, DeletedAt);
    }
}
