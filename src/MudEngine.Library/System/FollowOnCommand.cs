using MudEngine.Library.Domain.System;
namespace MudEngine.Library.System;

public class FollowOnCommand : CommandRequestDto
{
    public FollowOnCommand(Guid commandId, Guid connectionId, string commandLine)
    {
        CommandId = commandId;
        ConnectionId = connectionId;
        CommandLine = commandLine;
    }
    public Guid CommandId { get; private set; }
}