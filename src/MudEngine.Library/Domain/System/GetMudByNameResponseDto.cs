namespace MudEngine.Library.Domain.System;

public class GetMudByNameResponseDto
{
    public int EntityId { get; set; }
    public string? Name { get; set; }
    public string? LoginScreen { get; set; }
    public string? MessageOfTheDay { get; set; }
}