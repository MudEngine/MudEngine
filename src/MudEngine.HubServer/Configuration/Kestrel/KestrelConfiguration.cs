using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
namespace MudEngine.HubServer.Configuration.Kestrel;

public static class KestrelConfiguration
{
    public static void AddKestrelConfiguration(this WebApplicationBuilder builder)
    {
        builder.Services.AddGrpc(options => { options.Interceptors.Add<ExceptionInterceptor>(); });
        builder.Services.AddCors(o => o.AddPolicy("AllowAll", corsPolicyBuilder =>
        {
            corsPolicyBuilder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
        }));
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Listen(IPAddress.Loopback, 5000,
                listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; });
        });
    }
}