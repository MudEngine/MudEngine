﻿using MudEngine.Library.System;
namespace MudEngine.Library.Commands.Basic;

[Command("9d992665-662d-4730-9943-e89557ff946f")]
public class Say : BaseCommand, ICommand
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
        var say = "\"" + arguments[..1].ToUpper() + arguments[1..] + "\"[CR]";
        AddMessage("You say, " + say);
        AddMessage(player.Name! + " says, " + say, room.Entities, player);
        return Response;
    }
}