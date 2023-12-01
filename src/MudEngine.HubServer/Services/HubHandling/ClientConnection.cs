using Grpc.Core;
using MudEngine.Proto;
namespace MudEngine.HubServer.Services.HubHandling;

public class ClientConnection(CancellationTokenSource cancellationTokenSource,
    IServerStreamWriter<ClientResponseMessage> response)
{
    private readonly object _lock = new();
    private DateTime _lastMessageReceived = DateTime.Now;
    public IServerStreamWriter<ClientResponseMessage> Response { get; } = response;
    public DateTime LastMessageReceived
    {
        get => _lastMessageReceived;
        set
        {
            using var threadLock = new ThreadLock();
            if (threadLock.Lock(_lock))
            {
                _lastMessageReceived = value;
            }
        }
    }
    public void Close()
    {
        cancellationTokenSource.Cancel();
    }
}