using Microsoft.Data.SqlClient;
namespace MyPortfolio.DataAccess.Connections;
public sealed class SqlConnectionFactory(string connectionString)
{
    public SqlConnection Create() => new(connectionString);
}
