using Grpc.Core;
using MudEngine.Proto;
namespace MudEngine.HubServer.Services.HubHandling;

public class CommandEngine(IServerStreamWriter<CommandRequestMessage> response)
{
    private readonly object _lock = new();
    private volatile bool _processing;
    public IServerStreamWriter<CommandRequestMessage> Response { get; } = response;
    public bool Processing
    {
        get => _processing;
        set
        {
            using var threadLock = new ThreadLock();
            if (threadLock.Lock(_lock))
            {
                _processing = value;
            }
        }
    }
}