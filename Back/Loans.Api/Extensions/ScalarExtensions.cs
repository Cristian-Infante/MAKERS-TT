using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

namespace Loans.Api.Extensions;

public static class ScalarExtensions
{
    public static IServiceCollection AddScalarDocs(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Makers Loans API", Version = "v1" });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
        return services;
    }

    public static WebApplication MapScalarDocs(this WebApplication app)
    {
        app.UseSwagger();
        app.MapScalarApiReference(options =>
        {
            options.WithTitle("Makers Loans API");
            options.WithOpenApiRoutePattern("/swagger/v1/swagger.json");
            options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
        });
        return app;
    }
}
