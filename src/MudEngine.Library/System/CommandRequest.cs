using MudEngine.Database.DataTransferObjects.System;
using MudEngine.Database.Interfaces;
namespace MudEngine.Library.System;

public class CommandRequest : CommandRequestDto
{
    public CommandRequest(IDatabaseRepository databaseRepository, Guid connectionId, string commandLine, CancellationToken token)
    {
        ConnectionId = connectionId;
        CommandLine = commandLine;
        DatabaseRepository = databaseRepository;
        Token = token;
    }
    public IDatabaseRepository DatabaseRepository { get; private set; }
    public CancellationToken Token { get; private set; }
}