using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using Grpc.Core;
using MudEngine.CommandServer.Compiler;
using MudEngine.Library.Domain.System;
using MudEngine.Library.System;
using MudEngine.Proto;
using ClientMessageType = MudEngine.Proto.ClientMessageType;
using CommandResponseMessage = MudEngine.Proto.CommandResponseMessage;
using IDatabaseRepository = MudEngine.Library.Interfaces.IDatabaseRepository;
namespace MudEngine.CommandServer.Services.CommandHandling;

public class CommandEngine(ILogger<CommandEngine> _logger,
    MudEngineMessageService.MudEngineMessageServiceClient _client,
    IDatabaseRepository _databaseRepository,
    CollectibleAssemblyLoadContext _collectibleAssemblyLoadContext)
{
    private readonly ConcurrentDictionary<Guid, ICommand> _commands = new();
    private CancellationTokenSource? _commandEngineTokenSource;
    public string EngineId { get; } = Guid.NewGuid().ToString("N");
    private static Dictionary<Guid, ICommand> GetCommandsFromAssembly(Assembly assembly)
    {
        var commands = new Dictionary<Guid, ICommand>();
        foreach (var type in assembly
                     .GetTypes()
                     .Where(m =>
                         m.GetCustomAttributes(typeof(CommandAttribute), false).Length > 0
                         && m.GetInterfaces().Any(i => i is {IsPublic: true, Name: "ICommand"}))
                     .ToList())
        {
            var commandId = ((CommandAttribute) Attribute.GetCustomAttribute(type, typeof(CommandAttribute))!)
                .CommandId;
            commands.Add(commandId, (Activator.CreateInstance(type) as ICommand)!);
        }
        return commands;
    }
    private async Task ProcessCommand(CommandResponseMessageList messageList, Guid commandId, Guid connectionId,
        string commandLine, CancellationToken token)
    {
        if (commandId == Guid.Empty)
        {
            return;
        }
        _logger.LogWarning("USER: [{EngineId}] [{connectionId}] [{text}]",
            EngineId, connectionId.ToString("N"), commandLine);
        if (!_commands.ContainsKey(commandId))
        {
            // Load
        }
        if (!_commands.TryGetValue(commandId, out var command))
        {
            return;
        }
        var commandResponse = command.Execute(new CommandRequest(_databaseRepository, connectionId,
            commandLine, token));
        foreach (var responseMessage in commandResponse.ResponseMessages)
        {
            messageList.ResponseMessages.Add(new CommandResponseMessage
            {
                Cid = responseMessage.ConnectionId.ToString("N"),
                Message = new ClientResponseMessage
                {
                    Type = (ClientMessageType) responseMessage.MessageType,
                    Text = responseMessage.Text.ToString()
                }
            });
        }
        foreach (var followOnCommand in commandResponse.FollowOnCommands)
        {
            await ProcessCommand(messageList, followOnCommand.CommandId, followOnCommand.ConnectionId,
                followOnCommand.CommandLine!, token).ConfigureAwait(false);
        }
    }
    private async Task ProcessCommandRequestMessage(CommandRequestMessage commandRequestMessage,
        IAsyncStreamWriter<CommandResponseMessageList> requestStream,
        CancellationToken token)
    {
        var messageList = new CommandResponseMessageList();
        try
        {
            var message = commandRequestMessage.Message;
            var connectionId = !string.IsNullOrWhiteSpace(commandRequestMessage.Cid) ? Guid.Parse(commandRequestMessage.Cid) : Guid.Empty;
            switch (message.Type)
            {
                case ClientMessageType.System:
                    _logger.LogWarning("SYSTEM: [{EngineId}] [{connectionId}] [{text}]",
                        EngineId, commandRequestMessage.Cid, message.Text);
                    if (message.Text.StartsWith("ClientConnected"))
                    {
                        var newConnectionCommandId = await _databaseRepository.OnNewConnection(
                            new OnNewConnectionRequestDto
                            {
                                ConnectionId = connectionId,
                                AdditionalData = message.Text
                            }, token).ConfigureAwait(false);
                        await ProcessCommand(messageList, newConnectionCommandId, connectionId, message.Text, token)
                            .ConfigureAwait(false);
                    }
                    if (message.Text.StartsWith("ClientDisconnected"))
                    {
                        _ = await _databaseRepository.OnDisconnect(connectionId, token)
                            .ConfigureAwait(false);
                    }
                    if (message.Text.StartsWith("OnStartup"))
                    {
                        _ = await _databaseRepository.OnStartup(token)
                            .ConfigureAwait(false);
                        messageList.ResponseMessages.Add(new CommandResponseMessage
                        {
                            Cid = string.Empty,
                            Message = new ClientResponseMessage
                            {
                                Type = ClientMessageType.System,
                                Text = "OnStartupComplete"
                            }
                        });
                    }
                    break;
                case ClientMessageType.Gmcp:
                    _logger.LogWarning("GMCP: [{EngineId}] [{connectionId}] [{text}]",
                        EngineId, commandRequestMessage.Cid, message.Text);
                    break;
                case ClientMessageType.Mssp:
                    _logger.LogWarning("MSSP: [{EngineId}] [{connectionId}] [{text}]",
                        EngineId, commandRequestMessage.Cid, message.Text);
                    var msspData = await _databaseRepository.GetMssp(message.Text, token)
                        .ConfigureAwait(false);
                    messageList.ResponseMessages.Add(new CommandResponseMessage
                    {
                        Cid = commandRequestMessage.Cid,
                        Message = new ClientResponseMessage
                        {
                            Type = ClientMessageType.Mssp,
                            Text = JsonSerializer.Serialize(msspData)
                        }
                    });
                    break;
                case ClientMessageType.User:
                default:
                    var commandId = await _databaseRepository.OnUserCommand(new CommandRequestDto
                    {
                        ConnectionId = connectionId,
                        CommandLine = message.Text
                    }, token).ConfigureAwait(false);
                    await ProcessCommand(messageList, commandId, connectionId, message.Text, token)
                        .ConfigureAwait(false);
                    break;
            }
        }
        catch (RpcException e) when (e.StatusCode == StatusCode.Cancelled)
        {
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
        }
        catch (Exception)
        {
            _logger.LogTrace("ProcessCommandRequestMessage: {engineId}", EngineId);
        }
        finally
        {
            await requestStream.WriteAsync(messageList, token).ConfigureAwait(false);
        }
    }
    public async Task Start(string serviceId, CancellationToken commandServiceToken)
    {
        _commandEngineTokenSource = new CancellationTokenSource();
        var linkedTokenSource =
            CancellationTokenSource.CreateLinkedTokenSource(commandServiceToken,
                _commandEngineTokenSource.Token);
        var token = linkedTokenSource.Token;
        var commands = GetCommandsFromAssembly(typeof(ICommand).Assembly);
        foreach (var kvp in commands)
        {
            _ = _commands.TryAdd(kvp.Key, kvp.Value);
        }
        try
        {
            _logger.LogWarning("Started: CommandEngine [{engineId}]", EngineId);
            var commandMessages = _client.CommandMessages(new Metadata
            {
                {"ServiceId", serviceId},
                {"EngineId", EngineId}
            }, null, token);
            await foreach (var commandRequestMessage in commandMessages.ResponseStream
                               .ReadAllAsync(token).ConfigureAwait(false))
            {
                await ProcessCommandRequestMessage(commandRequestMessage,
                    commandMessages.RequestStream, token).ConfigureAwait(false);
            }
        }
        catch (RpcException e) when (e.StatusCode == StatusCode.Cancelled)
        {
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
        }
        catch (Exception)
        {
            _logger.LogTrace("Disconnected: {engineId}", EngineId);
        }
        finally
        {
            _logger.LogWarning("Disconnected: {engineId}", EngineId);
            await _commandEngineTokenSource.CancelAsync().ConfigureAwait(false);
        }
    }
    public void Stop()
    {
        _logger.LogInformation("CommandEngine_Stop");
        _commandEngineTokenSource?.Cancel();
        _collectibleAssemblyLoadContext.Unload();
        _commands.Clear();
    }
}