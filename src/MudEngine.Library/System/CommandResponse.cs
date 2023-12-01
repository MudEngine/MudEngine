using System.Diagnostics.CodeAnalysis;
namespace MudEngine.Library.System;

public class CommandResponse
{
    [SuppressMessage("Style", "IDE0028:Simplify collection initialization", Justification = "Pending Resharper")]
    public List<CommandResponseMessage> ResponseMessages { get; } = new();
    [SuppressMessage("Style", "IDE0028:Simplify collection initialization", Justification = "Pending Resharper")]
    public List<FollowOnCommand> FollowOnCommands { get; } = new();
}