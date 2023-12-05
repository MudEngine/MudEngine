using MudEngine.Library.Domain.Enum;

namespace MudEngine.Library.Domain.Mud;

public class GetRoomExitsResponseDto
{
    public int SourceId { get; set; }
    public int DestinationId { get; set; }
    public string? PrimaryAlias { get; set; }
    public int RoomExitVisibilityId { get; set; }
    public RoomExitVisibility RoomExitVisibility => (RoomExitVisibility)RoomExitVisibilityId;
}