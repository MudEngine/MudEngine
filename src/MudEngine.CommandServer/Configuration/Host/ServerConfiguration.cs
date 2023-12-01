using Serilog;
namespace MudEngine.CommandServer.Configuration.Host;

public class ServerConfiguration
{
    public ServerConfiguration(ConfigurationManager configuration)
    {
        try
        {
            HubServerUri = new Uri(configuration.GetValue<string>("HubServerUri") ??
                                   "http://localhost:5000");
        }
        catch (Exception e)
        {
            Log.Information(e, "ServerConfiguration_HubServerUri");
            HubServerUri = new Uri("http://localhost:5000");
        }
        try
        {
            MinContexts = int.Parse(configuration.GetValue<string>("MinContexts") ?? "1");
        }
        catch (Exception e)
        {
            Log.Information(e, "ServerConfiguration_MinContexts");
            MinContexts = 1;
        }
    }
    public Uri HubServerUri { get; }
    public int MinContexts { get; }
}