using MudEngine.Database.DataTransferObjects.Enums.Enum;
namespace MudEngine.Database.DataTransferObjects.Mud;

public class GetRoomExitsResponseDto
{
    public int SourceId { get; set; }
    public int DestinationId { get; set; }
    public string? PrimaryAlias { get; set; }
    public int RoomExitVisibilityId { get; set; }
    public RoomExitVisibility RoomExitVisibility => (RoomExitVisibility)RoomExitVisibilityId;
}