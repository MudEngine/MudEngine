using Microsoft.AspNetCore.Builder;
using MudEngine.Proto;
namespace MudEngine.TelnetServer.Configuration.Hub;

public static class HubConfiguration
{
    public static void AddHubConfiguration(this WebApplicationBuilder builder)
    {
        builder.Services.AddGrpcClient<MudEngineMessageService.MudEngineMessageServiceClient>(o =>
        {
            o.Address = new Uri(builder.Configuration.GetValue<string>("HubServerUri") ??
                                "http://localhost:5000");
        });
    }
}