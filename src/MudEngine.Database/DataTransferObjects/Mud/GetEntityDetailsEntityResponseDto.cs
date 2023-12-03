using MudEngine.Database.DataTransferObjects.Base;
using MudEngine.Database.DataTransferObjects.Enums.Enum;
namespace MudEngine.Database.DataTransferObjects.Mud;

public class GetEntityDetailsEntityResponseDto : EntityDto
{
    public int EntityTypeId { get; set; }
    public EntityType EntityType => (EntityType) EntityTypeId;
    public bool IsLiving { get; set; }
}