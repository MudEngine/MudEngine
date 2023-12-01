using Grpc.Core;
using MudEngine.Proto;
namespace MudEngine.HubServer.Services;

public class MessageService(HubService hubService) : MudEngineMessageService.MudEngineMessageServiceBase
{
    public override async Task ClientMessages(IAsyncStreamReader<ClientRequestMessage> request,
        IServerStreamWriter<ClientResponseMessage> response, ServerCallContext context)
    {
        var headers = context.RequestHeaders;
        var serviceId = headers.GetValue("ServiceId");
        var connectionId = headers.GetValue("ConnectionId");
        var localEndPoint = headers.GetValue("LocalEndPoint") ?? string.Empty;
        var remoteEndPoint = headers.GetValue("RemoteEndPoint") ?? string.Empty;
        if (serviceId is null || connectionId is null)
        {
            return;
        }
        await hubService.OnClientConnection(serviceId, connectionId, localEndPoint, remoteEndPoint, 
                request, response, context.CancellationToken)
            .ConfigureAwait(false);
    }
    public override async Task CommandMessages(IAsyncStreamReader<CommandResponseMessageList> request,
        IServerStreamWriter<CommandRequestMessage> response, ServerCallContext context)
    {
        var headers = context.RequestHeaders;
        var serviceId = headers.GetValue("ServiceId");
        var engineId = headers.GetValue("EngineId");
        if (serviceId is null || engineId is null)
        {
            return;
        }
        await hubService.OnCommandConnection(serviceId, engineId, request, response, context.CancellationToken)
            .ConfigureAwait(false);
    }
    public override async Task ControlMessages(IAsyncStreamReader<ControlMessage> request,
        IServerStreamWriter<ControlMessage> response, ServerCallContext context)
    {
        var headers = context.RequestHeaders;
        var serviceId = headers.GetValue("ServiceId");
        if (serviceId is null)
        {
            return;
        }
        await hubService.OnControlConnection(serviceId, request, response, context.CancellationToken)
            .ConfigureAwait(false);
    }
}