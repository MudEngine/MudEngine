namespace MudEngine.Library.Domain.Base;

public class Entity
{
    public Guid ConnectionId { get; set; }
    public int EntityId { get; set; }
    public string? Name { get; set; }
}