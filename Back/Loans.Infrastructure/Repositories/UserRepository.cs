using Dapper;
using Loans.Application.Interfaces;
using Loans.Domain.Entities;
using Loans.Domain.Enums;
using Loans.Infrastructure.Data;

namespace Loans.Infrastructure.Repositories;

public class UserRepository(UnitOfWork unitOfWork) : IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email)
    {
        const string sql = """
            SELECT Id, Email, PasswordHash, FullName, Role, CreatedAt, IsDeleted, DeletedAt
            FROM Users
            WHERE Email = @Email AND IsDeleted = 0
            """;
        var row = await unitOfWork.Connection.QueryFirstOrDefaultAsync<UserRow>(
            sql, new { Email = email.ToLowerInvariant() }, unitOfWork.Transaction);
        return row?.ToUser();
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        const string sql = """
            SELECT Id, Email, PasswordHash, FullName, Role, CreatedAt, IsDeleted, DeletedAt
            FROM Users
            WHERE Id = @Id AND IsDeleted = 0
            """;
        var row = await unitOfWork.Connection.QueryFirstOrDefaultAsync<UserRow>(
            sql, new { Id = id }, unitOfWork.Transaction);
        return row?.ToUser();
    }

    public async Task AddAsync(User user)
    {
        const string sql = """
            INSERT INTO Users (Id, Email, PasswordHash, FullName, Role, CreatedAt, IsDeleted)
            VALUES (@Id, @Email, @PasswordHash, @FullName, @Role, @CreatedAt, @IsDeleted)
            """;
        await unitOfWork.Connection.ExecuteAsync(sql, new
        {
            user.Id,
            user.Email,
            user.PasswordHash,
            user.FullName,
            Role = user.Role.ToString(),
            user.CreatedAt,
            user.IsDeleted
        }, unitOfWork.Transaction);
    }

    public async Task<bool> ExistsAsync(string email)
    {
        const string sql = "SELECT COUNT(1) FROM Users WHERE Email = @Email AND IsDeleted = 0";
        var count = await unitOfWork.Connection.ExecuteScalarAsync<int>(
            sql, new { Email = email.ToLowerInvariant() }, unitOfWork.Transaction);
        return count > 0;
    }

    private sealed class UserRow
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public User ToUser() => User.Reconstitute(
            Id, Email, PasswordHash, FullName,
            Enum.Parse<UserRole>(Role), CreatedAt, IsDeleted, DeletedAt);
    }
}
