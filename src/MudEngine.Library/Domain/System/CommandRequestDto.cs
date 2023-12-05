namespace MudEngine.Library.Domain.System;

public class CommandRequestDto
{
    public Guid ConnectionId { get; set; }
    public string? CommandLine { get; set; }
}