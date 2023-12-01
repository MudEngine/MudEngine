namespace MudEngine.Database.DataTransferObjects.Mud;

public class FindLocalEntityRequestDto
{
    public int EntityId { get; set; }
    public string? SearchText { get; set; }
}