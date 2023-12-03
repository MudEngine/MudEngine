using MudEngine.Library.System;
namespace MudEngine.Library.Commands.Basic;

[Command("a211ca5b-aeef-461f-95fd-63bb3993e5a7")]
public class Emote : BaseCommand, ICommand
{
    public override CommandResponse Execute(CommandRequest Request)
    {
        base.Execute(Request);
        var arguments = Arguments().Trim();
        if (string.IsNullOrWhiteSpace(arguments))
        {
            return Response;
        }
        if (!char.IsPunctuation(arguments[^1]))
        {
            arguments += ".";
        }
        var player = ThisPlayer();
        var room = GetEntityDetails(player.RoomId);
        var emote = player.Name! + " " + arguments + "[CR]";
        AddMessage("You emote: " + emote);
        AddMessage(emote, room.Entities, player);
        return Response;
    }
}