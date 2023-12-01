using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting.WindowsServices;
using MudEngine.TelnetServer.Configuration.Host;
using MudEngine.TelnetServer.Configuration.Hub;
using MudEngine.TelnetServer.Configuration.Kestrel;
using MudEngine.TelnetServer.Configuration.Logging;
using MudEngine.TelnetServer.Processors;
using MudEngine.TelnetServer.Services;
using MudEngine.TelnetServer.Services.ConnectionHandling;
using Serilog;
using Serilog.Events;
namespace MudEngine.TelnetServer;

internal static class Program
{
    private static TelnetService? _telnetService;
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
            var builder = WebApplication.CreateBuilder(
                new WebApplicationOptions
                {
                    Args = args,
                    ContentRootPath = WindowsServiceHelpers.IsWindowsService()
                        ? AppContext.BaseDirectory
                        : default,
                    ApplicationName = Process.GetCurrentProcess().ProcessName
                });
            builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly(), true);
            builder.AddLoggingConfiguration();
            builder.AddHubConfiguration();
            builder.AddKestrelConfiguration(async connection =>
            {
                await _telnetService!.OnNewConnection(connection).ConfigureAwait(false);
            });
            builder.Services.AddHostedService<ServiceHost>();
            builder.Services.AddSingleton<TelnetService>();
            builder.Services.AddTransient<InterpretAsCommandProcessor>();
            builder.Services.AddTransient<TelnetDataProcessor>();
            builder.Services.AddTransient<TelnetConnection>();
            var host = builder.Build();
            _telnetService = host.Services.GetRequiredService<TelnetService>();
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