using System.Data;
using System.Data.SqlClient;
namespace MudEngine.CommandServer.Configuration.Database;

public static class DatabaseConfiguration
{
    public static void AddDatabaseConfiguration(this HostApplicationBuilder builder)
    {
        var connectionString =
            builder.Configuration.GetConnectionString("MudEngine") ?? string.Empty;
        builder.Services.AddSingleton<Func<IDbConnection>>(_ => () => new SqlConnection(connectionString));
    }
}