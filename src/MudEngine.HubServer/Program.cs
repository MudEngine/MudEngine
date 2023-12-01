using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting.WindowsServices;
using MudEngine.HubServer.Configuration.Host;
using MudEngine.HubServer.Configuration.Kestrel;
using MudEngine.HubServer.Configuration.Logging;
using MudEngine.HubServer.Services;
using Serilog;
using Serilog.Events;
namespace MudEngine.HubServer;

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
            var builder = WebApplication.CreateBuilder(
                new WebApplicationOptions
                {
                    Args = args,
                    ContentRootPath = WindowsServiceHelpers.IsWindowsService()
                        ? AppContext.BaseDirectory
                        : default,
                    ApplicationName = Process.GetCurrentProcess().ProcessName
                });
            builder.AddLoggingConfiguration();
            builder.AddKestrelConfiguration();
            builder.Services.AddHostedService<ServiceHost>();
            builder.Services.AddSingleton<HubService>();
            var host = builder.Build();
            host.UseRouting();
            host.UseCors();
            host.UseGrpcWeb();
            host.MapGrpcService<MessageService>()
                .RequireCors("AllowAll")
                .EnableGrpcWeb();
            host.MapGet("/", () => "Mud Engine Hub Server");
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