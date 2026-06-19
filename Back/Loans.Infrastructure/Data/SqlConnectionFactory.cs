using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Loans.Infrastructure.Data;

public class SqlConnectionFactory(IConfiguration configuration)
{
    public SqlConnection CreateConnection() =>
        new(configuration.GetConnectionString("DefaultConnection"));
}
