using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Hosting.WindowsServices;
using MudEngine.CommandServer.Compiler;
using MudEngine.CommandServer.Configuration.Database;
using MudEngine.CommandServer.Configuration.Host;
using MudEngine.CommandServer.Configuration.Hub;
using MudEngine.CommandServer.Configuration.Logging;
using MudEngine.CommandServer.Services;
using MudEngine.CommandServer.Services.CommandHandling;
using Serilog;
using Serilog.Events;
using DatabaseRepository = MudEngine.Library.Repositories.DatabaseRepository;
using IDatabaseRepository = MudEngine.Library.Interfaces.IDatabaseRepository;
namespace MudEngine.CommandServer;

internal static class Program
{
    private static void Main(string[] args)
    {
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
                ContentRootPath = WindowsServiceHelpers.IsWindowsService()
                    ? AppContext.BaseDirectory
                    : default,
                ApplicationName = Process.GetCurrentProcess().ProcessName
            });
            builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly(), true);
            var serverConfiguration = new ServerConfiguration(builder.Configuration);
            builder.Services.AddSingleton(serverConfiguration);
            builder.AddLoggingConfiguration();
            builder.AddHubConfiguration(serverConfiguration);
            builder.AddDatabaseConfiguration();
            builder.Services.AddHostedService<ServiceHost>();
            builder.Services.AddSingleton<CommandService>();
            builder.Services.AddSingleton<CompilerService>();
            builder.Services.AddTransient<CommandEngine>();
            builder.Services.AddTransient<CollectibleAssemblyLoadContext>();
            builder.Services.AddTransient<IDatabaseRepository, DatabaseRepository>();
            var host = builder.Build();
            host.Run();
        }
        catch (Exception e)
        {
            Log.Fatal(e, "Terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
        Thread.Sleep(500);
        Environment.Exit(0);
    }
}