namespace MudEngine.Database.DataTransferObjects.Transient;

public class SetConnectionAccountRequestDto
{
    public Guid ConnectionId { get; set; }
    public Guid AccountId { get; set; }
}