namespace MudEngine.Database.DataTransferObjects.Transient;

public class ConnectionCommandListRequestDto
{
    public Guid ConnectionId { get; set; }
    public string? CommandListName { get; set; }
}