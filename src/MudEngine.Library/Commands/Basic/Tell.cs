using MudEngine.Library.System;
namespace MudEngine.Library.Commands.Basic;

[Command("31c561d3-a60a-4383-a6d2-cb7bcdde4391")]
public class Tell : BaseCommand, ICommand
{
    public override CommandResponse Execute(CommandRequest Request)
    {
        base.Execute(Request);
        var arguments = Arguments().Trim();
        if (string.IsNullOrWhiteSpace(arguments) || !arguments.Contains(' '))
        {
            AddMessage("You have nothing to tell![CR]");
            return Response;
        }
        var possibleTarget = arguments[..arguments.IndexOf(' ')];
        arguments = arguments[(arguments.IndexOf(' ') + 1)..].Trim();
        if (string.IsNullOrWhiteSpace(arguments))
        {
            AddMessage("You have nothing to tell![CR]");
            return Response;
        }
        var player = ThisPlayer();
        var target = GetPlayerByName(possibleTarget);
        if (target.EntityId <= 0)
        {
            AddMessage("Tell who?[CR]");
            return Response;
        }
        if (!char.IsPunctuation(arguments[^1]))
        {
            arguments += ".";
        }
        var tell = $"\"{arguments[..1].ToUpper()}{arguments[1..]}\"";
        AddMessage($"You tell {target.Name}, {tell}[CR]");
        AddMessage($"{player.Name!} tells you, {tell}[CR]", target);
        return Response;
    }
}