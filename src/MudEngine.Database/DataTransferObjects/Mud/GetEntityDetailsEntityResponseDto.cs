using MudEngine.Database.DataTransferObjects.Enums.Enum;
namespace MudEngine.Database.DataTransferObjects.Mud;

public class GetEntityDetailsEntityResponseDto
{
    public int EntityId { get; set; }
    public int EntityTypeId { get; set; }
    public EntityType EntityType => (EntityType) EntityTypeId;
    public string? Name { get; set; }
    public bool IsLiving { get; set; }
    public Guid ConnectionId { get; set; }
}