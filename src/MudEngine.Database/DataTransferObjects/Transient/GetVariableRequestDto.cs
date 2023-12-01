namespace MudEngine.Database.DataTransferObjects.Transient;

public class GetVariableRequestDto
{
    public Guid ConnectionId { get; set; }
    public string? Name { get; set; }
}