using MudEngine.Library.Domain.Base;
using MudEngine.Library.System;
namespace MudEngine.Library.Commands.Basic;

[Command("a8b4ecd9-2c84-4a81-9c85-0b7458c24e26")]
public class Whisper : BaseCommand, ICommand
{
    public override CommandResponse Execute(CommandRequest Request)
    {
        base.Execute(Request);
        var arguments = Arguments().Trim();
        if (string.IsNullOrWhiteSpace(arguments))
        {
            AddMessage("You have nothing to whisper![CR]");
            return Response;
        }
        var player = ThisPlayer();
        Entity? target = null;
        if (arguments.Contains(" to ", StringComparison.OrdinalIgnoreCase))
        {
            var possibleTarget = arguments[(arguments.LastIndexOf(" to ", StringComparison.OrdinalIgnoreCase) + 3)..];
            var targetId = FindLocalEntity(player.EntityId, possibleTarget);
            if (targetId > 0)
            {
                target = GetEntityDetails(targetId);
                arguments = arguments[..arguments.LastIndexOf(" to ", StringComparison.OrdinalIgnoreCase)];
            }
        }
        if (target is null && arguments.Contains(' '))
        {
            var possibleTarget = arguments[..arguments.IndexOf(' ')];
            arguments = arguments[(arguments.IndexOf(' ') + 1)..].Trim();
            target = GetPlayerByName(possibleTarget);
        }
        if (string.IsNullOrWhiteSpace(arguments))
        {
            AddMessage("You have nothing to whisper![CR]");
            return Response;
        }
        if (target is null)
        {
            AddMessage("Whisper to who?[CR]");
            return Response;
        }
        if (!char.IsPunctuation(arguments[^1]))
        {
            arguments += ".";
        }
        var whisper = $"\"{arguments[..1].ToUpper()}{arguments[1..]}\"";
        AddMessage($"You whisper, {whisper} to {target.Name}.[CR]");
        AddMessage($"{player.Name!} whispers, {whisper}[CR]", target);
        return Response;
    }
}