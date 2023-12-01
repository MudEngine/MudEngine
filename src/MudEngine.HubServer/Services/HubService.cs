using System.Collections.Concurrent;
using Grpc.Core;
using MudEngine.HubServer.Processors;
using MudEngine.HubServer.Services.HubHandling;
using MudEngine.Proto;
namespace MudEngine.HubServer.Services;

public class HubService(ILogger<HubService> logger)
{
    private readonly ConcurrentDictionary<string, ClientConnection> _clientConnections = new();
    private readonly ConcurrentDictionary<string, CommandEngine> _commandEngines = new();
    private readonly ConcurrentDictionary<string, ControlConnection> _controlConnections = new();
    private readonly object _lock = new();
    private readonly ManualResetEvent _mre = new(false);
    private readonly string _msspUptime = new DateTimeOffset(DateTime.UtcNow)
        .ToUnixTimeSeconds()
        .ToString();
    private readonly ConcurrentQueue<CommandRequestMessage> _requestMessages = new();
    private CancellationTokenSource? _hubServiceTokenSource;
    private volatile bool _isStarted;
    private void AddClientRequestMessage(string connectionId, ClientRequestMessage message)
    {
        var commandRequestMessage = new CommandRequestMessage
        {
            Cid = connectionId,
            Message = message
        };
        switch (message.Type)
        {
            case ClientMessageType.Mssp:
                commandRequestMessage.Message.Text = _msspUptime;
                break;
            case ClientMessageType.User:
                commandRequestMessage.Message.Text = message.Text.Utf8ToAscii();
                break;
            case ClientMessageType.Gmcp:
            case ClientMessageType.System:
            default:
                break;
        }
        try
        {
            _requestMessages.Enqueue(commandRequestMessage);
            _mre.Set();
        }
        catch (Exception e)
        {
            logger.LogTrace(e, "AddClientRequestMessage");
        }
    }
    public async Task OnClientConnection(string serviceId,
        string connectionId, string localEndPoint, string remoteEndPoint,
        IAsyncStreamReader<ClientRequestMessage> request,
        IServerStreamWriter<ClientResponseMessage> response,
        CancellationToken token)
    {
        var tokenSource = new CancellationTokenSource();
        var linkedTokenSource =
            CancellationTokenSource.CreateLinkedTokenSource(token,
                tokenSource.Token);
        try
        {
            logger.LogWarning("Connected: [{ServiceId}] [{ConnectionId}]",
                serviceId, connectionId);
            var clientConnection = new ClientConnection(linkedTokenSource, response);
            if (_clientConnections.TryAdd(connectionId, clientConnection))
            {
                AddClientRequestMessage(connectionId, new ClientRequestMessage
                {
                    Type = ClientMessageType.System,
                    Text = "ClientConnected " + localEndPoint + "|" + remoteEndPoint
                });
                await foreach (var message in request.ReadAllAsync(linkedTokenSource.Token).ConfigureAwait(false))
                {
                    clientConnection.LastMessageReceived = DateTime.Now;
                    AddClientRequestMessage(connectionId, message);
                }
            }
        }
        catch (RpcException e) when (e.StatusCode == StatusCode.Cancelled)
        {
        }
        catch (Exception)
        {
            logger.LogTrace("Disconnected: {ConnectionId}", connectionId);
        }
        finally
        {
            await linkedTokenSource.CancelAsync().ConfigureAwait(false);
            AddClientRequestMessage(connectionId, new ClientRequestMessage
            {
                Type = ClientMessageType.System,
                Text = "ClientDisconnected"
            });
            _ = _clientConnections.TryRemove(connectionId, out _);
            logger.LogWarning("Disconnected: {ConnectionId}", connectionId);
        }
    }
    public async Task OnCommandConnection(string serviceId,
        string engineId,
        IAsyncStreamReader<CommandResponseMessageList> request,
        IServerStreamWriter<CommandRequestMessage> response,
        CancellationToken token)
    {
        try
        {
            logger.LogWarning("Engine Added: [{ServiceId}] [{EngineId}]",
                serviceId, engineId);
            var commandEngine = new CommandEngine(response);
            if (_commandEngines.TryAdd(engineId, commandEngine))
            {
                _mre.Set();
                await foreach (var messageList in request.ReadAllAsync(token).ConfigureAwait(false))
                {
                    foreach (var responseMessage in messageList.ResponseMessages)
                    {
                        if (string.IsNullOrWhiteSpace(responseMessage.Cid))
                        {
                            await OnDaemonResponse(responseMessage.Message, token).ConfigureAwait(false);
                        }
                        else
                        {
                            try
                            {
                                if (_clientConnections.TryGetValue(responseMessage.Cid, out var clientConnection))
                                {
                                    await clientConnection.Response.WriteAsync(responseMessage.Message, token)
                                        .ConfigureAwait(false);
                                }
                            }
                            catch (Exception e)
                            {
                                logger.LogTrace(e, "OnCommandConnection");
                            }
                        }
                    }
                    commandEngine.Processing = false;
                    _mre.Set();
                }
            }
        }
        catch (RpcException e) when (e.StatusCode == StatusCode.Cancelled)
        {
        }
        catch (Exception e)
        {
            logger.LogTrace(e, "OnCommandConnection: {engineId}", engineId);
        }
        finally
        {
            _ = _commandEngines.TryRemove(engineId, out _);
            logger.LogWarning("Engine Removed: {engineId}", engineId);
        }
    }
    public async Task OnControlConnection(string serviceId,
        IAsyncStreamReader<ControlMessage> request,
        IServerStreamWriter<ControlMessage> response,
        CancellationToken token)
    {
        try
        {
            logger.LogWarning("Control Connection: Established [{serviceId}]", serviceId);
            await foreach (var message in request.ReadAllAsync(token).ConfigureAwait(false))
            {
                if (message.Type != ControlMessageType.Register)
                {
                    logger.LogWarning("ControlMessage: [{serviceId}] [{type}] [{text}]",
                        serviceId, message.Type.ToString("G"), message.Text);
                    return;
                }
                logger.LogWarning("Register: [{serviceId}] [{text}]",
                    serviceId, message.Text);
                using var threadLock = new ThreadLock();
                if (!threadLock.Lock(_lock))
                {
                    break;
                }
                var controlConnection = new ControlConnection(_isStarted || message.Text == "CommandService", response);
                if (!_controlConnections.TryAdd(serviceId, controlConnection))
                {
                    break;
                }
                if (controlConnection.Registered)
                {
                    await response.WriteAsync(new ControlMessage
                    {
                        Type = ControlMessageType.Registered,
                        Text = string.Empty
                    }, token).ConfigureAwait(false);
                }
            }
        }
        catch (RpcException e) when (e.StatusCode == StatusCode.Cancelled)
        {
        }
        catch (Exception e)
        {
            logger.LogTrace(e, "OnControlConnection");
        }
        finally
        {
            _ = _controlConnections.TryRemove(serviceId, out _);
            logger.LogWarning("Control Connection: Terminated [{serviceId}]", serviceId);
        }
    }
    private async Task OnDaemonResponse(ClientResponseMessage responseMessage, CancellationToken token)
    {
        logger.LogWarning("OnDaemonResponse: [{type}] [{text}]", responseMessage.Type.ToString("G"), responseMessage.Text);
        if (responseMessage is {Type: ClientMessageType.System, Text: "OnStartupComplete"})
        {
            while (true)
            {
                using var threadLock = new ThreadLock();
                if (!threadLock.Lock(_lock))
                {
                    continue;
                }
                _isStarted = true;
                foreach (var connection in _controlConnections.Where(y => !y.Value.Registered))
                {
                    connection.Value.Registered = true;
                    await connection.Value.Response.WriteAsync(new ControlMessage
                    {
                        Type = ControlMessageType.Registered,
                        Text = string.Empty
                    }, token).ConfigureAwait(false);
                }
                break;
            }
        }
        else
        {
            logger.LogTrace("OnCommandConnection [{type}] [{text}]", responseMessage.Type, responseMessage.Text);
        }
    }
    public async Task Start(CancellationToken stoppingToken)
    {
        _hubServiceTokenSource = new CancellationTokenSource();
        try
        {
            var linkedTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(stoppingToken,
                    _hubServiceTokenSource.Token);
            var token = linkedTokenSource.Token;
            logger.LogWarning("Hub started at: {time}", DateTimeOffset.Now);
            AddClientRequestMessage(string.Empty, new ClientRequestMessage
            {
                Type = ClientMessageType.System,
                Text = "OnStartup"
            });
            while (true)
            {
                token.ThrowIfCancellationRequested();
                try
                {
                    _mre.Reset();
                    if (!_requestMessages.IsEmpty)
                    {
                        var commandEngine = _commandEngines.Values.FirstOrDefault(ce => ce.Processing == false);
                        if (commandEngine is not null)
                        {
                            _mre.Set();
                            if (_requestMessages.TryDequeue(out var commandRequestMessage))
                            {
                                commandEngine.Processing = true;
                                await commandEngine.Response.WriteAsync(commandRequestMessage, token)
                                    .ConfigureAwait(false);
                            }
                        }
                    }
                    //var idleConnections =
                    //    _clientConnections.Values.Where(c => c.LastMessageReceived < DateTime.Now.AddMinutes(-2));
                    _mre.WaitOne(2500);
                }
                catch (Exception e)
                {
                    logger.LogTrace(e, "HubService_Start");
                }
            }
        }
        catch (Exception e)
        {
            logger.LogTrace(e, "HubService_Start");
        }
        finally
        {
            await _hubServiceTokenSource.CancelAsync().ConfigureAwait(false);
        }
    }
    public void Stop()
    {
        logger.LogInformation("HubService_Stop");
        _hubServiceTokenSource?.Cancel();
    }
}