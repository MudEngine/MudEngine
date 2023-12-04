using MudEngine.Library.System;
namespace MudEngine.Library.Commands.Basic;

[Command("baa4ad89-36f7-4928-a59a-8ae620ec6564")]
public class Unknown : BaseCommand, ICommand
{
    public override CommandResponse Execute(CommandRequest Request)
    {
        base.Execute(Request);
        if (string.IsNullOrWhiteSpace(Request.CommandLine))
        {
            return Response;
        }
        if (Request.CommandLine.StartsWith('\"'))
        {
            AddUserCommand($"say {Request.CommandLine[1..].Trim()}");
            return Response;
        }
        if (Request.CommandLine.StartsWith(':'))
        {
            AddUserCommand($"emote {Request.CommandLine[1..].Trim()}");
            return Response;
        }
        var player = ThisPlayer();
        var exits = GetRoomExits(player.RoomId);
        var validExit = exits.FirstOrDefault(e =>
            e.PrimaryAlias!.Equals(Request.CommandLine, StringComparison.OrdinalIgnoreCase));
        if (validExit is not null)
        {
            AddFollowOnCommand("System", "move", validExit.PrimaryAlias);
            return Response;
        }
        var aliases = GetPlayerAliases(player.EntityId).ToList();
        var alias = aliases.FirstOrDefault(e =>
            e.Alias!.Equals(Request.CommandLine, StringComparison.OrdinalIgnoreCase));
        if (alias is not null)
        {
            AddUserCommand(alias.Replacement);
            return Response;
        }
        alias = aliases.FirstOrDefault(e =>
            Request.CommandLine.StartsWith(e.Alias!));
        if (alias is not null)
        {
            AddUserCommand(Request.CommandLine.Replace(alias.Alias!, alias.Replacement ?? string.Empty,
                StringComparison.OrdinalIgnoreCase));
            return Response;
        }
        AddMessage(IsDirection(Command()) 
            ? "You can't go that way.[CR]" 
            : "Unknown command.[CR]");
        return Response;
    }
}