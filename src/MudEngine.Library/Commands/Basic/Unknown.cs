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
        var player = ThisPlayer();
        var exits = GetRoomExits(player.RoomId);
        var validExit = exits.FirstOrDefault(e =>
            e.PrimaryAlias!.Equals(Request.CommandLine, StringComparison.InvariantCultureIgnoreCase));
        if (validExit is not null)
        {
            AddFollowOnCommand("System", "move", validExit.PrimaryAlias);
            return Response;
        }
        var aliases = GetPlayerAliases(player.EntityId).ToList();
        var alias = aliases.FirstOrDefault(e =>
            e.Alias!.Equals(Request.CommandLine, StringComparison.InvariantCultureIgnoreCase));
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
                StringComparison.InvariantCultureIgnoreCase));
            return Response;
        }
        switch (Command().ToLower())
        {
            case "east":
            case "northeast":
            case "north":
            case "northwest":
            case "west":
            case "southwest":
            case "south":
            case "southeast":
            case "in":
            case "out":
                AddMessage("You can't go that way.[CR]");
                break;
            default:
                AddMessage("Unknown command.[CR]");
                break;
        }
        return Response;
    }
}