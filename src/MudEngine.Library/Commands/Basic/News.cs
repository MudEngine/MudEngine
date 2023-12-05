using MudEngine.Library.System;
namespace MudEngine.Library.Commands.Basic;

[Command("11ca3b8b-62d1-4811-ae6c-f6cd812c8e2c")]
public class News : BaseCommand, ICommand
{
    public override CommandResponse Execute(CommandRequest Request)
    {
        base.Execute(Request);
        var mud = Mud();
        if (!string.IsNullOrWhiteSpace(mud.News))
        {
            AddMessage($"{mud.News}[CR]");
        }
        return Response;
    }
}