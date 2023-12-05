using MudEngine.Library.Domain.Base;
using MudEngine.Library.Domain.Enum;

namespace MudEngine.Library.Domain.Mud;

public class GetEntityDetailsEntityResponseDto : Entity
{
    public int EntityTypeId { get; set; }
    public EntityType EntityType => (EntityType) EntityTypeId;
    public bool IsLiving { get; set; }
}