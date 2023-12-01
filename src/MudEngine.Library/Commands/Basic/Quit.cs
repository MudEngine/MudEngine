using MudEngine.Library.System;
namespace MudEngine.Library.Commands.Basic;

[Command("20b61361-c4e3-425f-b50b-dcb4cd06c463")]
public class Quit : BaseCommand, ICommand
{
    public override CommandResponse Execute(CommandRequest Request)
    {
        base.Execute(Request);
        AddMessage("Goodbye.[CR]");
        AddSystemMessage("Disconnect");
        return Response;
    }
}