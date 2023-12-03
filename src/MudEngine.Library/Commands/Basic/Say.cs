using MudEngine.Library.System;
namespace MudEngine.Library.Commands.Basic;

[Command("9d992665-662d-4730-9943-e89557ff946f")]
public class Say : BaseCommand, ICommand
{
    public override CommandResponse Execute(CommandRequest Request)
    {
        base.Execute(Request);

        return Response;
    }
}