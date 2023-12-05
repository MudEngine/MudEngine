using MudEngine.Library.Domain.Base;
using MudEngine.Library.System;
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
            AddMessage("You have nothing to say![CR]");
            return Response;
        }
        var player = ThisPlayer();
        Entity? target = null;
        if (arguments.Contains(" to ", StringComparison.OrdinalIgnoreCase))
        {
            var possibleTarget = arguments[(arguments.LastIndexOf(" to ", StringComparison.OrdinalIgnoreCase) + 3)..];
            var targetId = FindLocalEntity(player.EntityId, possibleTarget);
            if(targetId > 0)
            {
                target = GetEntityDetails(targetId);
                arguments = arguments[..arguments.LastIndexOf(" to ", StringComparison.OrdinalIgnoreCase)];
            }
        }
        if (!char.IsPunctuation(arguments[^1]))
        {
            arguments += ".";
        }
        var say = "\"" + arguments[..1].ToUpper() + arguments[1..] + "\"";
        if(target is null)
        {
            AddMessage("You say, " + say + "[CR]");
            AddMessage(player.Name! + " says, " + say + "[CR]", GetLivingInRoom(player), player);
        }
        else
        {
            AddMessage($"You say, {say} to {target.Name}.[CR]");
            AddMessage($"{player.Name!} says, {say} to you.[CR]", target);
            AddMessage($"{player.Name!} says, {say} to {target.Name}.[CR]", GetLivingInRoom(player), new[] {player, target});
        }
        return Response;
    }
}