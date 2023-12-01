namespace MudEngine.Library.System;

public interface ICommand
{
    CommandResponse Execute(CommandRequest Request);
}