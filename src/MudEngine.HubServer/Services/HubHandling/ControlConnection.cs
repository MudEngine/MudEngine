using Grpc.Core;
using MudEngine.Proto;
namespace MudEngine.HubServer.Services.HubHandling;

public class ControlConnection(bool registered, IServerStreamWriter<ControlMessage> response)
{
    public IServerStreamWriter<ControlMessage> Response { get; } = response;
    public bool Registered { get; set; } = registered;
}