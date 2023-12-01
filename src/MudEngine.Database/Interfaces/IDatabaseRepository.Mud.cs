using MudEngine.Database.DataTransferObjects.Mud;
namespace MudEngine.Database.Interfaces;

public partial interface IDatabaseRepository
{
    Task<int> FindLocalEntity(FindLocalEntityRequestDto request, CancellationToken cancellationToken = default);
    Task<GetEntityDetailsResponseDto> GetEntityDetails(int entityId, CancellationToken cancellationToken = default);
    Task<IEnumerable<GetPlayerAliasesResponseDto>> GetPlayerAliases(int playerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<GetRoomExitsResponseDto>> GetRoomExits(int roomId, CancellationToken cancellationToken = default);
    Task<int> SetPlayerRoom(SetPlayerRoomRequestDto setPlayerRoomRequestDto, CancellationToken cancellationToken = default);
}