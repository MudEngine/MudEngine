using MudEngine.Database.DataTransferObjects.Transient;
namespace MudEngine.Database.Interfaces;

public partial interface IDatabaseRepository
{
    Task<int> AddConnectionCommandList(ConnectionCommandListRequestDto connectionCommandListRequestDto, CancellationToken cancellationToken = default);
    Task<GetConnectionPlayerResponseDto> GetConnectionPlayer(Guid connectionId, CancellationToken cancellationToken = default);
    Task<string> GetConnectionVariable(GetVariableRequestDto connectionVariableRequestDto, CancellationToken cancellationToken = default);
    Task<int> RemoveConnectionCommandList(ConnectionCommandListRequestDto connectionCommandListRequestDto, CancellationToken cancellationToken = default);
    Task<IEnumerable<SetConnectionAccountResponseDto>> SetConnectionAccount(SetConnectionAccountRequestDto request, CancellationToken cancellationToken = default);
    Task<IEnumerable<Guid>> SetConnectionPlayer(SetConnectionPlayerRequestDto request, CancellationToken cancellationToken = default);
    Task<int> UpsertConnectionVariable(UpsertVariableRequestDto upsertVariableRequestDto, CancellationToken cancellationToken = default);
}