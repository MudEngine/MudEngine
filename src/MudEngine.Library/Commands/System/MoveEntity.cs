using MudEngine.Library.System;
namespace MudEngine.Library.Commands.System;

[Command("1415a2de-1587-437f-8164-8a2843c1864f")]
public class MoveEntity : BaseCommand, ICommand
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
        var validExit = exits.FirstOrDefault(e => e.PrimaryAlias!.Equals(Request.CommandLine, StringComparison.InvariantCultureIgnoreCase));
        if (validExit is null)
        {
            return Response;
        }
        SetPlayerRoom(player.EntityId, validExit.DestinationId);
        AddFollowOnCommand("Basic", "look", "look here");
        return Response;
    }
}