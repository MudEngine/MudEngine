namespace MudEngine.Library.Domain.Mud;

public class FindLocalEntityRequestDto
{
    public int EntityId { get; set; }
    public string? SearchText { get; set; }
    public int Index { get; set; }
}