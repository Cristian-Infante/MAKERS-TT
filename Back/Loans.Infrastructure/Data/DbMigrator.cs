using DbUp;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Loans.Infrastructure.Data;

public class DbMigrator(IConfiguration configuration)
{
    public void Migrate()
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")!;

        EnsureDatabase.For.SqlDatabase(connectionString);

        var upgrader = DeployChanges.To
            .SqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .LogToConsole()
            .Build();

        var result = upgrader.PerformUpgrade();
        if (!result.Successful)
            throw new Exception("Database migration failed.", result.Error);
    }
}
