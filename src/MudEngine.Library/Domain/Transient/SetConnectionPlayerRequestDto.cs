namespace MudEngine.Library.Domain.Transient;

public class SetConnectionPlayerRequestDto
{
    public Guid ConnectionId { get; set; }
    public int PlayerId { get; set; }
}