using System.Collections.Concurrent;
using System.Net;
using Grpc.Core;
using Microsoft.AspNetCore.Connections;
using MudEngine.Proto;
using MudEngine.TelnetServer.Services.ConnectionHandling;
namespace MudEngine.TelnetServer.Services;

public class TelnetService(ILogger<TelnetService> logger,
    IServiceProvider serviceProvider,
    MudEngineMessageService.MudEngineMessageServiceClient client)
{
    private readonly ConcurrentDictionary<string, DateTime> _bannedIps = new();
    private readonly string _serviceId = Guid.NewGuid().ToString("N");
    private bool _registered;
    private CancellationTokenSource? _telnetServiceTokenSource;
    private bool IsBanned(IPEndPoint endPoint)
    {
        var address = endPoint.Address.ToString();
        return _bannedIps.ContainsKey(address);
    }
    public async Task OnNewConnection(ConnectionContext connection)
    {
        if (connection.RemoteEndPoint is null
            || _telnetServiceTokenSource is null
            || _telnetServiceTokenSource.IsCancellationRequested
            || !_registered
            || IsBanned((IPEndPoint) connection.RemoteEndPoint))
        {
            connection.Abort();
            return;
        }
        connection.ConnectionId = Guid.NewGuid().ToString("N");
        var telnetConnection = ActivatorUtilities.CreateInstance<TelnetConnection>(serviceProvider, connection);
        await telnetConnection.AcceptConnection(_serviceId, _telnetServiceTokenSource.Token).ConfigureAwait(false);
    }
    public async Task Start(CancellationToken stoppingToken)
    {
        _registered = false;
        _telnetServiceTokenSource = new CancellationTokenSource();
        try
        {
            var linkedTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(stoppingToken,
                    _telnetServiceTokenSource.Token);
            logger.LogInformation("Started: TelnetService");
            var controlMessages = client.ControlMessages(new Metadata
            {
                {"ServiceId", _serviceId}
            }, null, linkedTokenSource.Token);
            await Task.WhenAll(Task.Run(async () =>
                {
                    await foreach (var message in controlMessages.ResponseStream.ReadAllAsync(linkedTokenSource.Token)
                                       .ConfigureAwait(false))
                    {
                        logger.LogWarning("ControlMessage: [{type}] [{text}]",
                            message.Type.ToString("G"), message.Text);
                        switch (message.Type)
                        {
                            case ControlMessageType.Registered:
                                _registered = true;
                                break;
                            case ControlMessageType.BanIp:
                                _ = _bannedIps.TryAdd(message.Text, DateTime.Now);
                                break;
                            case ControlMessageType.UnBanIp:
                                _ = _bannedIps.TryRemove(message.Text, out _);
                                break;
                            case ControlMessageType.Register:
                            default:
                                break;
                        }
                    }
                }, stoppingToken),
                Task.Run(async () =>
                {
                    await controlMessages.RequestStream.WriteAsync(new ControlMessage
                    {
                        Type = ControlMessageType.Register,
                        Text = "TelnetService"
                    }, linkedTokenSource.Token).ConfigureAwait(false);
                }, stoppingToken)).ConfigureAwait(false);
        }
        catch (RpcException e) when (e.StatusCode == StatusCode.Cancelled)
        {
        }
        catch (Exception)
        {
            logger.LogWarning("TelnetService_OnControlMessageReceived: Disconnected");
        }
        finally
        {
            await _telnetServiceTokenSource.CancelAsync().ConfigureAwait(false);
        }
    }
    public void Stop()
    {
        logger.LogInformation("TelnetService_Stop");
        _telnetServiceTokenSource?.Cancel();
    }
}