using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting;
using Serilog;
namespace MudEngine.TelnetServer.Configuration.Kestrel;

public static class KestrelConfiguration
{
    public static void AddKestrelConfiguration(this WebApplicationBuilder builder,
        Func<ConnectionContext, Task> onNewConnection)
    {
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Limits.KeepAliveTimeout = TimeSpan.MaxValue;
            options.Limits.MinRequestBodyDataRate = null;
            options.Limits.MinResponseDataRate = null;
            var validEndpoints = new Dictionary<string, IPEndPoint>();
            foreach (var endPointConfiguration in builder.Configuration
                         .GetSection("TelnetServer:EndPoints")
                         .GetChildren())
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(endPointConfiguration.Value) ||
                        !IPEndPoint.TryParse(endPointConfiguration.Value!, out var ipEndPoint))
                    {
                        continue;
                    }
                    _ = validEndpoints.TryAdd(endPointConfiguration.Value!, ipEndPoint);
                }
                catch (Exception e)
                {
                    Log.Error(e, "AddKestrelConfiguration");
                }
            }
            if (validEndpoints.Count == 0)
            {
                validEndpoints.Add("0.0.0.0:4000", new IPEndPoint(IPAddress.Parse("0.0.0.0"), 4000));
            }
            foreach (var validEndpoint in validEndpoints)
            {
                options.Listen(validEndpoint.Value,
                    listenOptions => listenOptions.Run(onNewConnection));
            }
        });
    }
}