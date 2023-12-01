using MudEngine.CommandServer.Configuration.Host;
using MudEngine.Proto;
namespace MudEngine.CommandServer.Configuration.Hub;

public static class HubConfiguration
{
    public static void AddHubConfiguration(this HostApplicationBuilder builder,
        ServerConfiguration serverConfiguration)
    {
        builder.Services.AddGrpcClient<MudEngineMessageService.MudEngineMessageServiceClient>(o =>
        {
            o.Address = serverConfiguration.HubServerUri;
        });
    }
}