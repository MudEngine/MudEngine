namespace MudEngine.Library.Domain.Transient;

public class GetVariableRequestDto
{
    public Guid ConnectionId { get; set; }
    public string? Name { get; set; }
}