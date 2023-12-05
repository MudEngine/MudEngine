using MudEngine.Library.Domain.System;
using IDatabaseRepository = MudEngine.Library.Interfaces.IDatabaseRepository;
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