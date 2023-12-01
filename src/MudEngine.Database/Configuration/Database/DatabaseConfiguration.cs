using System.Data;
using Microsoft.Data.SqlClient;
namespace MudEngine.Database.Configuration.Database;

public static class DatabaseConfiguration
{
    public static void AddDatabaseConfiguration(this HostApplicationBuilder builder)
    {
        var connectionString =
            builder.Configuration.GetConnectionString("MudEngine") ?? string.Empty;
        builder.Services.AddSingleton<Func<IDbConnection>>(_ => () => new SqlConnection(connectionString));
    }
}