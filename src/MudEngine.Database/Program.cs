using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using MudEngine.Database.Configuration.Database;
using MudEngine.Database.Seeding.Models;
using Serilog;
using Serilog.Events;
using Assembly = System.Reflection.Assembly;
using EntityType = MudEngine.Database.DataTransferObjects.Enums.Enum.EntityType;
namespace MudEngine.Database;

internal static class Program
{
    private static void Main(string[] args)
    {
        if (Environment.GetCommandLineArgs().Contains("migrations") ||
            Environment.GetCommandLineArgs().Contains("database"))
        {
            return;
        }
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Debug(LogEventLevel.Information)
            .WriteTo.Console(LogEventLevel.Warning)
            .CreateLogger();
        try
        {
            var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
            {
                Args = args,
                ContentRootPath = AppContext.BaseDirectory,
                ApplicationName = Process.GetCurrentProcess().ProcessName
            });
            builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly(), true);
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(dispose: true);
            builder.AddDatabaseConfiguration();
            builder.Services.AddDbContext<MudEngineContext>();
            var host = builder.Build();
            Log.Warning("Configuring database...");
            host.Seed("Configuration");
            Log.Warning("Seeding schemas...");
            host.SeedSchema("Enum");
            host.SeedSchema("Mud");
            host.SeedSchema("System");
            host.SeedSchema("Transient");
            Log.Warning("Attempting migration...");
            var migrationContext = host.Services.GetRequiredService<MudEngineContext>();
            migrationContext.Database.Migrate();
            Log.Warning("Migration complete.");
            Log.Warning("Seeding stored procedures...");
            host.Seed("StoredProcedures");
            foreach (var dtoEnum in Assembly.GetAssembly(typeof(EntityType))!
                         .GetTypes()
                         .Where(t => t.IsEnum && t.Namespace!
                             .Contains("DataTransferObjects.Enums"))
                         .ToList())
            {
                host.SeedEnum(dtoEnum);
            }
            Log.Warning("Seeding Triggers...");
            host.Seed("Triggers");
            Log.Warning("Seeding Data...");
            host.Seed("Data");
            Log.Warning("Complete.");
        }
        catch (Exception exception)
        {
            Log.Fatal(exception, "Terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
        Thread.Sleep(500);
        Environment.Exit(0);
    }
}