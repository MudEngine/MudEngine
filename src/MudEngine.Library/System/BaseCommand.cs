using MudEngine.Database.DataTransferObjects.Mud;
using MudEngine.Database.DataTransferObjects.System;
using MudEngine.Database.DataTransferObjects.Transient;
using MudEngine.Database.Interfaces;
using MudEngine.Library.System.PartOfSpeechTagging;
namespace MudEngine.Library.System;

public abstract class BaseCommand
{
    private static Corpus? _corpus;
    private string _arguments = null!;
    private string _command = null!;
    private string _commandLine = null!;
    private Guid _connectionId = Guid.Empty;
    private IDatabaseRepository _databaseRepository = null!;
    private CancellationToken _token;
    protected CommandResponse Response { get; private set; } = null!;
    protected void AddCommandList(string commandListName)
    {
        if (string.IsNullOrWhiteSpace(commandListName) || commandListName.Length > 78)
        {
            return;
        }
        _ = _databaseRepository.AddConnectionCommandList(
            new ConnectionCommandListRequestDto
            {
                ConnectionId = _connectionId,
                CommandListName = commandListName
            }, _token).GetAwaiter().GetResult();
    }
    protected void AddFollowOnCommand(string commandListName, string primaryAlias, string? commandLine)
    {
        if (string.IsNullOrWhiteSpace(commandListName) || commandListName.Length > 78 ||
            string.IsNullOrWhiteSpace(primaryAlias) || primaryAlias.Length > 78)
        {
            return;
        }
        var commandId =
            _databaseRepository.GetCommandByListAndAlias(new GetCommandByListAndAliasRequestDto
            {
                CommandListName = commandListName,
                PrimaryAlias = primaryAlias
            }, _token).GetAwaiter().GetResult();
        if (commandId == Guid.Empty)
        {
            return;
        }
        Response.FollowOnCommands.Add(new FollowOnCommand(commandId, _connectionId, commandLine ?? string.Empty));
    }
    protected void AddMessage(string text)
    {
        AddMessage(_connectionId, ClientMessageType.User, text);
    }
    private void AddMessage(Guid connectionId, ClientMessageType messageType, string text)
    {
        var responseMessage = Response.ResponseMessages.FirstOrDefault(rm =>
            rm.ConnectionId == connectionId && rm.MessageType == messageType);
        if (responseMessage is null)
        {
            Response.ResponseMessages.Add(new CommandResponseMessage(connectionId, messageType, text));
        }
        else
        {
            responseMessage.Text.Append(text);
        }
    }
    protected void AddSystemMessage(string text)
    {
        AddMessage(_connectionId, ClientMessageType.System, text);
    }
    protected void AddUserCommand(string? commandLine)
    {
        var commandId = _databaseRepository.OnUserCommand(new CommandRequestDto
        {
            ConnectionId = _connectionId,
            CommandLine = commandLine
        }, _token).GetAwaiter().GetResult();
        if (commandId == Guid.Empty)
        {
            return;
        }
        Response.FollowOnCommands.Add(new FollowOnCommand(commandId, _connectionId, commandLine ?? string.Empty));
    }
    protected string Command()
    {
        return _command;
    }
    protected string ConnectionVariables(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > 78)
        {
            return string.Empty;
        }
        return _databaseRepository.GetConnectionVariable(new GetVariableRequestDto
        {
            ConnectionId = _connectionId,
            Name = name
        }, _token).GetAwaiter().GetResult();
    }
    public virtual CommandResponse Execute(CommandRequest Request)
    {
        _connectionId = Request.ConnectionId;
        _databaseRepository = Request.DatabaseRepository;
        Response = new CommandResponse();
        _token = Request.Token;
        _commandLine = Request.CommandLine ?? string.Empty;
        _command = _commandLine.Contains(' ') ? _commandLine[.._commandLine.IndexOf(' ')] : _commandLine;
        _arguments = _commandLine.Contains(' ') ? _commandLine[(_commandLine.IndexOf(' ') + 1)..] : string.Empty;
        return Response;
    }
    private int FindLocalEntity(int entityId, string searchText, int index = 1)
    {
        if (entityId == 0 || string.IsNullOrWhiteSpace(searchText) || searchText.Length > 78)
        {
            return 0;
        }
        return _databaseRepository.FindLocalEntity(new FindLocalEntityRequestDto
        {
            EntityId = entityId,
            SearchText = searchText,
            Index = index
        }, _token).GetAwaiter().GetResult();
    }
    protected static string FormatArray(string[] array)
    {
        return array.Length switch
        {
            0 => string.Empty,
            1 => array.First() + ".",
            2 => string.Join(" and ", array) + ".",
            _ => string.Join(", ", array.Take(array.Length - 1))
                 + " and " + array.Last() + "."
        };
    }
    protected GetEntityDetailsResponseDto GetEntityDetails(int entityId)
    {
        return entityId == 0
            ? new GetEntityDetailsResponseDto()
            : _databaseRepository.GetEntityDetails(entityId, _token).GetAwaiter().GetResult();
    }
    protected IEnumerable<GetPlayerAliasesResponseDto> GetPlayerAliases(int playerId)
    {
        return _databaseRepository.GetPlayerAliases(playerId, _token).GetAwaiter().GetResult();
    }
    protected IEnumerable<GetRoomExitsResponseDto> GetRoomExits(int roomId)
    {
        return _databaseRepository.GetRoomExits(roomId, _token).GetAwaiter().GetResult();
    }
    protected int IdentifySubject()
    {
        var player = ThisPlayer();
        return IdentifySubject(player.EntityId, player.RoomId, _arguments);
    }
    private int IdentifySubject(int entityId, int roomId, string arguments)
    {
        _corpus ??= new Corpus();
        var subjectId = 0;
        var partsOfSpeech = Tagger.Tag(_corpus, arguments)
            .Where(s => !s.Type!.Equals("IN")
                        && !s.Type!.Equals("DT")
                        && !s.Type!.Equals("JJ"))
            .ToList();
        var searchText = partsOfSpeech.Count > 0
            ? string.Join(' ', partsOfSpeech.Select(s => s.Token))
            : arguments;
        switch (searchText.ToLower())
        {
            case "me":
                subjectId = entityId;
                break;
            case "":
            case "around":
            case "here":
            case "place":
            case "room":
                subjectId = roomId;
                break;
            default:
                var subjects = partsOfSpeech
                    .Where(s => s.Type!.StartsWith('N'))
                    .ToList();
                if (subjects.Count > 0)
                {
                    var subject = subjects.First();
                    subjectId = FindLocalEntity(entityId, subject.Token!, subject.Index);
                }
                break;
        }
        return subjectId;
    }
    protected GetMudByNameResponseDto Mud(string mudName = "")
    {
        if (mudName.Length > 78)
        {
            mudName = mudName[..78];
        }
        return _databaseRepository.GetMudByName(mudName, _token).GetAwaiter().GetResult();
    }
    protected void RemoveCommandList(string commandListName)
    {
        if (string.IsNullOrWhiteSpace(commandListName) || commandListName.Length > 78)
        {
            return;
        }
        _ = _databaseRepository.RemoveConnectionCommandList(
            new ConnectionCommandListRequestDto
            {
                ConnectionId = _connectionId,
                CommandListName = commandListName
            }, _token).GetAwaiter().GetResult();
    }
    protected IEnumerable<SetConnectionAccountResponseDto> SetConnectionAccount(Guid accountId)
    {
        return _databaseRepository.SetConnectionAccount(new SetConnectionAccountRequestDto
        {
            ConnectionId = _connectionId,
            AccountId = accountId
        }, _token).GetAwaiter().GetResult();
    }
    protected void SetConnectionPlayer(int playerId)
    {
        if (playerId == 0)
        {
            return;
        }
        var connectionsToDisconnect = _databaseRepository.SetConnectionPlayer(
            new SetConnectionPlayerRequestDto
            {
                ConnectionId = _connectionId,
                PlayerId = playerId
            }, _token).GetAwaiter().GetResult();
        foreach (var connectionToDisconnect in connectionsToDisconnect)
        {
            AddMessage(connectionToDisconnect, ClientMessageType.System, "Disconnect");
        }
    }
    protected void SetConnectionVariable(string name, string value)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > 78)
        {
            return;
        }
        _ = _databaseRepository.UpsertConnectionVariable(new UpsertVariableRequestDto
        {
            ConnectionId = _connectionId,
            Name = name,
            Value = value
        }, _token).GetAwaiter().GetResult();
    }
    protected void SetPlayerRoom(int playerId, int destinationRoomId)
    {
        _ = _databaseRepository.SetPlayerRoom(new SetPlayerRoomRequestDto
        {
            PlayerId = playerId,
            DestinationRoomId = destinationRoomId
        }, _token).GetAwaiter().GetResult();
    }
    protected GetConnectionPlayerResponseDto ThisPlayer()
    {
        return _databaseRepository.GetConnectionPlayer(_connectionId, _token).GetAwaiter().GetResult();
    }
    protected Guid ValidateAccountPassword(string accountName, string password)
    {
        if (string.IsNullOrWhiteSpace(accountName) || accountName.Length > 78 || string.IsNullOrWhiteSpace(password))
        {
            return Guid.Empty;
        }
        var loginAccountDto = _databaseRepository.GetAccountForLogin(accountName, _token).GetAwaiter().GetResult();
        return !string.IsNullOrWhiteSpace(loginAccountDto.HashedPassword) &&
               BCrypt.Net.BCrypt.Verify(password, loginAccountDto.HashedPassword)
            ? loginAccountDto.AccountId
            : Guid.Empty;
    }
}