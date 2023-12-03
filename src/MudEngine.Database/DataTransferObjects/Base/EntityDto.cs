namespace MudEngine.Database.DataTransferObjects.Base;

public class EntityDto
{
    public Guid ConnectionId { get; set; }
    public int EntityId { get; set; }
    public string? Name { get; set; }
}