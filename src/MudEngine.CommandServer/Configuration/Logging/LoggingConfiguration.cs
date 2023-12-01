using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Hosting.WindowsServices;
using Serilog;
namespace MudEngine.CommandServer.Configuration.Logging;

public static class LoggingConfiguration
{
    public static void AddLoggingConfiguration(this HostApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        if (!Debugger.IsAttached && WindowsServiceHelpers.IsWindowsService())
        {
            builder.Logging.AddConsole();
            builder.Logging.AddEventLog(settings =>
            {
                settings.LogName = Assembly.GetExecutingAssembly()
                    .EntryPoint!.DeclaringType!.Namespace!
                    .Replace(".", string.Empty);
                if (string.IsNullOrEmpty(settings.SourceName))
                {
                    settings.SourceName = builder.Environment.ApplicationName;
                }
            });
            builder.Services.AddWindowsService();
        }
        else
        {
            builder.Logging.AddSerilog(dispose: true);
        }
    }
}