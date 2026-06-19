using Dapper;
using Loans.Infrastructure.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Loans.Api.HealthChecks;

public sealed class SqlHealthCheck(SqlConnectionFactory connectionFactory) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = connectionFactory.CreateConnection();
            await connection.ExecuteScalarAsync<int>("SELECT 1");
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(ex.Message);
        }
    }
}
