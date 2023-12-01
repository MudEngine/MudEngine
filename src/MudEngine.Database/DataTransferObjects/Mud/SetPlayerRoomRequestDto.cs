namespace MudEngine.Database.DataTransferObjects.Mud
{
    public class SetPlayerRoomRequestDto
    {
        public int PlayerId { get; set; }
        public int DestinationRoomId { get; set; }
    }
}
