namespace MudEngine.Library.Domain.System;

public class OnNewConnectionRequestDto()
{
    public Guid ConnectionId { get; set; }
    public string? AdditionalData { get; set; }
}