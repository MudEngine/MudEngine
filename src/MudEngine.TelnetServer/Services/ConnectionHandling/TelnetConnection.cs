using System.Text;
using System.Threading.Channels;
using Grpc.Core;
using Microsoft.AspNetCore.Connections;
using MudEngine.Proto;
using MudEngine.TelnetServer.Processors;
namespace MudEngine.TelnetServer.Services.ConnectionHandling;

public class TelnetConnection(ILogger<TelnetConnection> logger,
    MudEngineMessageService.MudEngineMessageServiceClient client,
    TelnetDataProcessor telnetDataProcessor,
    ConnectionContext connection)
{
    private const int MaxBufferSize = 4000;
    private readonly Channel<ClientRequestMessage> _channel = Channel.CreateBounded<ClientRequestMessage>(
        new BoundedChannelOptions(20)
        {
            FullMode = BoundedChannelFullMode.DropWrite,
            SingleReader = true,
            SingleWriter = true
        });
    private readonly ReadOnlyMemory<byte> _gmcpFooter = new byte[] {255, 240};
    private readonly ReadOnlyMemory<byte> _gmcpHeader = new byte[] {255, 250, 201};
    private readonly ReadOnlyMemory<byte> _onNewConnection = new byte[]
    {
        255, 251, 1, //   WILL echo
        255, 253, 3, //   DO suppress go ahead
        255, 252, 34, //  WONT linemode
        255, 251, 201, // WILL GMCP
        255, 251, 70, //  WILL MSSP
        255, 249 //       GO AHEAD
    };
    private readonly CancellationTokenSource _telnetConnectionTokenSource = new();
    public async Task AcceptConnection(string serviceId, CancellationToken telnetServiceToken)
    {
        var linkedTokenSource =
            CancellationTokenSource.CreateLinkedTokenSource(
                _telnetConnectionTokenSource.Token,
                telnetServiceToken);
        var token = linkedTokenSource.Token;
        try
        {
            logger.LogWarning("Connected: {ConnectionId}", connection.ConnectionId);
            var outputPipe = connection.Transport.Output;
            await outputPipe.WriteAsync(_onNewConnection, token).ConfigureAwait(false);
            var clientMessages = client.ClientMessages(new Metadata
            {
                {"ServiceId", serviceId},
                {"ConnectionId", connection.ConnectionId},
                {"LocalEndPoint", connection.LocalEndPoint?.ToString() ?? string.Empty},
                {"RemoteEndPoint", connection.RemoteEndPoint?.ToString() ?? string.Empty}
            }, null, token);
            await Task.WhenAll(Task.Run(async () =>
                {
                    await foreach (var message in clientMessages.ResponseStream.ReadAllAsync(linkedTokenSource.Token)
                                       .ConfigureAwait(false))
                    {
                        switch (message.Type)
                        {
                            case ClientMessageType.Gmcp:
                                await outputPipe.WriteAsync(_gmcpHeader, linkedTokenSource.Token).ConfigureAwait(false);
                                await outputPipe.WriteAsync(new ReadOnlyMemory<byte>(
                                        Encoding.UTF8.GetBytes(message.Text)),
                                    linkedTokenSource.Token).ConfigureAwait(false);
                                await outputPipe.WriteAsync(_gmcpFooter, linkedTokenSource.Token).ConfigureAwait(false);
                                break;

                            case ClientMessageType.Mssp:
                                logger.LogWarning("MSSP: [{text}]", message.Text);
                                await outputPipe.WriteAsync(message.Text.ToMssp(),
                                    linkedTokenSource.Token).ConfigureAwait(false);
                                break;
                            case ClientMessageType.System:
                                logger.LogWarning("System: [{text}]", message.Text);
                                if (message.Text.StartsWith("Disconnect"))
                                {
                                    await _telnetConnectionTokenSource.CancelAsync().ConfigureAwait(false);
                                }
                                break;
                            case ClientMessageType.User:
                            default:
                                logger.LogWarning("Received: [{text}]", message.Text);
                                await outputPipe.WriteAsync(new ReadOnlyMemory<byte>(
                                        Encoding.UTF8.GetBytes(message.Text
                                            .ReplaceAnsiTags())),
                                    linkedTokenSource.Token).ConfigureAwait(false);
                                break;
                        }
                    }
                }, telnetServiceToken),
                ReceiveDataFromTelnetClient(linkedTokenSource),
                SendDataToHubServer(clientMessages.RequestStream, token)).ConfigureAwait(false);
        }
        catch (RpcException e) when (e.StatusCode == StatusCode.Cancelled)
        {
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
        }
        catch (Exception e)
        {
            logger.LogTrace(e, "TelnetConnection");
        }
        finally
        {
            logger.LogWarning("Disconnected: {ConnectionId}", connection.ConnectionId);
            await _telnetConnectionTokenSource.CancelAsync().ConfigureAwait(false);
        }
    }
    private async Task ReceiveDataFromTelnetClient(CancellationTokenSource tokenSource)
    {
        var inputPipe = connection.Transport.Input;
        var outputPipe = connection.Transport.Output;
        var token = tokenSource.Token;
        while (true)
        {
            token.ThrowIfCancellationRequested();
            var result = await inputPipe.ReadAsync(token).ConfigureAwait(false);
            if (result.IsCompleted)
            {
                await tokenSource.CancelAsync().ConfigureAwait(false);
                break;
            }
            var buffer = result.Buffer;
            if (buffer.Length > MaxBufferSize)
            {
                logger.LogInformation("Exceeded buffer size constraint: [{data}]", buffer.Length.ToString());
                inputPipe.AdvanceTo(buffer.End);
                continue;
            }
            foreach (var segment in buffer)
            {
                token.ThrowIfCancellationRequested();
                if (segment.IsEmpty || segment.Length == 0)
                {
                    continue;
                }
                var processorResponse = await telnetDataProcessor.ProcessSegment(segment, token).ConfigureAwait(false);
                foreach (var iacCommand in processorResponse.Commands)
                {
                    await outputPipe.WriteAsync(iacCommand, token).ConfigureAwait(false);
                }
                foreach (var gmcp in processorResponse.GMCPRequests)
                {
                    await _channel.Writer.WriteAsync(new ClientRequestMessage
                    {
                        Type = ClientMessageType.Gmcp,
                        Text = Encoding.UTF8.GetString(gmcp)
                    }, token).ConfigureAwait(false);
                }
                foreach (var line in processorResponse.Lines)
                {
                    await _channel.Writer.WriteAsync(new ClientRequestMessage
                    {
                        Type = ClientMessageType.User,
                        Text = Encoding.UTF8.GetString(line)
                            .Replace("\r", string.Empty)
                            .Replace("\n", string.Empty)
                    }, token).ConfigureAwait(false);
                }
                if (processorResponse.ProvideServerStatus)
                {
                    await _channel.Writer.WriteAsync(new ClientRequestMessage
                    {
                        Type = ClientMessageType.Mssp
                    }, token).ConfigureAwait(false);
                }
            }
            token.ThrowIfCancellationRequested();
            inputPipe.AdvanceTo(buffer.End);
        }
    }
    private async Task SendDataToHubServer(IAsyncStreamWriter<ClientRequestMessage> hubServerClientRequestStream,
        CancellationToken token)
    {
        await foreach (var requestMessage in _channel.Reader.ReadAllAsync(token).ConfigureAwait(false))
        {
            await hubServerClientRequestStream.WriteAsync(requestMessage, token).ConfigureAwait(false);
        }
    }
}