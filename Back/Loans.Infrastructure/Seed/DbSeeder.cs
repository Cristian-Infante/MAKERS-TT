using Dapper;
using Loans.Application.Interfaces;
using Loans.Infrastructure.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Loans.Infrastructure.Seed;

public sealed class DbSeeder(
    SqlConnectionFactory connectionFactory,
    IPasswordHasher passwordHasher,
    IHostEnvironment environment,
    ILogger<DbSeeder> logger)
{
    public async Task SeedAsync()
    {
        if (!environment.IsDevelopment()) return;

        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        await SeedUserAsync(connection, "usuario@test.com", "123", "Usuario Test", "User");
        await SeedUserAsync(connection, "admin@test.com", "123", "Admin Test", "Admin");

        logger.LogInformation("Dev seed complete.");
    }

    private async Task SeedUserAsync(
        Microsoft.Data.SqlClient.SqlConnection connection,
        string email, string password, string fullName, string role)
    {
        const string existsSql = "SELECT COUNT(1) FROM Users WHERE Email = @Email";
        var exists = await connection.ExecuteScalarAsync<int>(existsSql, new { Email = email });
        if (exists > 0) return;

        const string insertSql = """
            INSERT INTO Users (Id, Email, PasswordHash, FullName, Role, CreatedAt, IsDeleted)
            VALUES (NEWID(), @Email, @PasswordHash, @FullName, @Role, GETUTCDATE(), 0)
            """;

        await connection.ExecuteAsync(insertSql, new
        {
            Email = email,
            PasswordHash = passwordHasher.Hash(password),
            FullName = fullName,
            Role = role
        });

        logger.LogInformation("Seeded user: {Email}", email);
    }
}
