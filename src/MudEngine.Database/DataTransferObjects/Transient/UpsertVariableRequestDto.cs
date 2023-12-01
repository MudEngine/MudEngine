namespace MudEngine.Database.DataTransferObjects.Transient;

public class UpsertVariableRequestDto
{
    public Guid ConnectionId { get; set; }
    public string? Name { get; set; }
    public string? Value { get; set; }
}