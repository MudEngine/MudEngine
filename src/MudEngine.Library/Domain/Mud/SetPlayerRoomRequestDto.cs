namespace MudEngine.Library.Domain.Mud;

public class SetPlayerRoomRequestDto
{
    public int PlayerId { get; set; }
    public int DestinationRoomId { get; set; }
}