using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Grpc.Core;
using MudEngine.CommandServer.Configuration.Host;
using MudEngine.CommandServer.Services.CommandHandling;
using MudEngine.Proto;
namespace MudEngine.CommandServer.Services;

public class CommandService(ILogger<CommandService> logger,
    IServiceProvider serviceProvider,
    MudEngineMessageService.MudEngineMessageServiceClient client,
    ServerConfiguration serverConfiguration)
{
    private readonly ConcurrentDictionary<string, CommandEngine> _commandEngines = new();
    private readonly string _serviceId = Guid.NewGuid().ToString("N");
    private CancellationTokenSource? _commandServiceTokenSource;
    private bool _registered;
    [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Pending Resharper")]
    public async Task Start(CancellationToken stoppingToken)
    {
        _registered = false;
        _commandServiceTokenSource = new CancellationTokenSource();
        try
        {
            var linkedTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(stoppingToken,
                    _commandServiceTokenSource.Token);
            var engineCount = serverConfiguration.MinContexts;
            logger.LogWarning("Started: CommandService [{serviceId}]", _serviceId);
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
                        if (message.Type == ControlMessageType.Registered)
                        {
                            _registered = true;
                        }
                    }
                }, stoppingToken),
                Task.Run(async () =>
                {
                    await controlMessages.RequestStream.WriteAsync(new ControlMessage
                    {
                        Type = ControlMessageType.Register,
                        Text = "CommandService"
                    }, linkedTokenSource.Token).ConfigureAwait(false);
                }, stoppingToken),
                Task.Run(() =>
                {
                    try
                    {
                        var token = linkedTokenSource.Token;
                        while (true)
                        {
                            token.ThrowIfCancellationRequested();
                            if (_registered)
                            {
                                if (_commandEngines.Count < engineCount)
                                {
                                    var commandEngine =
                                        ActivatorUtilities.CreateInstance<CommandEngine>(serviceProvider);
                                    if (_commandEngines.TryAdd(commandEngine.EngineId, commandEngine))
                                    {
                                        Task.Run(
                                            async () =>
                                            {
                                                await commandEngine.Start(_serviceId, token).ConfigureAwait(false);
                                            },
                                            token).ConfigureAwait(false);
                                    }
                                }
                                if (_commandEngines.Count > engineCount)
                                {
                                    var engineId = _commandEngines.Keys.LastOrDefault();
                                    if (engineId is not null)
                                    {
                                        if (_commandEngines.TryRemove(engineId, out var commandEngineToStop))
                                        {
                                            commandEngineToStop.Stop();
                                        }
                                    }
                                }
                            }
                            _ = WaitHandle.WaitAny(new[] { token.WaitHandle }, 2500);
                        }
                    }
                    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                    {
                    }
                    catch (Exception e)
                    {
                        logger.LogTrace(e, "CommandService_Maintain");
                    }
                }, stoppingToken)).ConfigureAwait(false);
        }
        catch (RpcException e) when (e.StatusCode == StatusCode.Cancelled)
        {
        }
        catch (Exception e)
        {
            logger.LogTrace(e, "CommandService_Start");
        }
        finally
        {
            await _commandServiceTokenSource.CancelAsync().ConfigureAwait(false);
            _commandEngines.Clear();
        }
    }
    public void Stop()
    {
        logger.LogInformation("CommandService_Stop");
        _commandServiceTokenSource?.Cancel();
    }
}