using DotNetEnv;
using Loans.Api.Extensions;
using Wolverine;
using Loans.Api.HealthChecks;
using Loans.Api.Middlewares;
using Loans.Api.Services;
using Loans.Application.Commands;
using Loans.Application.DependencyInjection;
using Loans.Infrastructure.Data;
using Loans.Infrastructure.DependencyInjection;
using Loans.Infrastructure.Seed;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

builder.UseWolverine(opts =>
{
    opts.Discovery.IncludeAssembly(typeof(LoginCommand).Assembly);
});

builder.Services.AddControllers();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddScoped<IAuthApiService, AuthApiService>();
builder.Services.AddScoped<ILoansApiService, LoansApiService>();

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddHealthChecks()
    .AddCheck<SqlHealthCheck>("sql", tags: ["ready"]);

builder.Services.AddScalarDocs();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<DbMigrator>().Migrate();

    if (app.Environment.IsDevelopment())
    {
        await scope.ServiceProvider.GetRequiredService<DbSeeder>().SeedAsync();
    }
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => false,
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
});

if (app.Environment.IsDevelopment())
{
    app.MapScalarDocs();
}

app.Run();
