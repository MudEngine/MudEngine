namespace MudEngine.Database.DataTransferObjects.Transient;

public class GetConnectionPlayerResponseDto
{
    public int EntityId { get; set; }
    public string? Name { get; set; }
    public int RoomId { get; set; }
}