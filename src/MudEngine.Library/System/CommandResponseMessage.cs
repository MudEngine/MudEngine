using System.Text;
using MudEngine.Library.Domain.System;
namespace MudEngine.Library.System;

public class CommandResponseMessage(Guid connectionId, ClientMessageType messageType, string text)
{
    public Guid ConnectionId { get; } = connectionId;
    public ClientMessageType MessageType { get; } = messageType;
    public StringBuilder Text { get; } = new (text);
}