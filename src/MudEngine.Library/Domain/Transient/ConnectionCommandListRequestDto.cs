namespace MudEngine.Library.Domain.Transient;

public class ConnectionCommandListRequestDto
{
    public Guid ConnectionId { get; set; }
    public string? CommandListName { get; set; }
}