using MudEngine.Library.System;
namespace MudEngine.Library.Commands.NewConnection;

[Command("ea0ad88f-fd70-46dc-b7ee-53f8b6339eb8")]
public class OnNewConnection : BaseCommand, ICommand
{
    public override CommandResponse Execute(CommandRequest Request)
    {
        base.Execute(Request);
        var mud = Mud();
        if (string.IsNullOrWhiteSpace(mud.LoginScreen))
        {
            AddMessage("[RED][BOLD]Connected[RESET].[CR]");
        }
        else
        {
            AddMessage(mud.LoginScreen + "[CR]");
        }
        AddCommandList("New Connections");
        AddFollowOnCommand("New Connections", "login", string.Empty);
        return Response;
    }
}