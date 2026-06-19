using Loans.Application.Interfaces;
using Loans.Infrastructure.Auth;
using Loans.Infrastructure.Caching;
using Loans.Infrastructure.Configuration;
using Loans.Infrastructure.Data;
using Loans.Infrastructure.Seed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Loans.Infrastructure.DependencyInjection;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtOptions>(opts =>
            configuration.GetSection(JwtOptions.SectionName).Bind(opts));

        services.AddMemoryCache();

        services.AddScoped<SqlConnectionFactory>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddSingleton<ICacheService, MemoryCacheService>();
        services.AddSingleton<ITokenService, JwtTokenService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        services.AddScoped<DbMigrator>();
        services.AddScoped<DbSeeder>();

        return services;
    }
}
