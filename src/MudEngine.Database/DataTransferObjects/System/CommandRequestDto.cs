namespace MudEngine.Database.DataTransferObjects.System;

public class CommandRequestDto
{
    public Guid ConnectionId { get; set; }
    public string? CommandLine { get; set; }
}