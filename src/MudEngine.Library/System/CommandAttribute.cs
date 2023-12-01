namespace MudEngine.Library.System;

[AttributeUsage(AttributeTargets.Class)]
public class CommandAttribute(string commandId) : Attribute
{
    public Guid CommandId { get; } = Guid.TryParse(commandId, out var guid) ? guid : Guid.Empty;
}