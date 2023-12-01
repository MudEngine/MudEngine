using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting.WindowsServices;
using Serilog;
namespace MudEngine.HubServer.Configuration.Logging;

public static class HubConfiguration
{
    public static void AddLoggingConfiguration(this WebApplicationBuilder builder)
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
            builder.Services.AddSingleton<IHostLifetime, WindowsServiceLifetime>();
            builder.Host.UseWindowsService();
        }
        else
        {
            builder.Logging.AddSerilog(dispose: true);
        }
    }
}