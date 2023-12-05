using MudEngine.Library.Domain.Base;
using MudEngine.Library.Domain.Transient;
namespace MudEngine.Library.Interfaces;

public partial interface IDatabaseRepository
{
    Task<int> AddConnectionCommandList(ConnectionCommandListRequestDto connectionCommandListRequestDto, CancellationToken cancellationToken = default);
    Task<Player> GetConnectionPlayer(Guid connectionId, CancellationToken cancellationToken = default);
    Task<string> GetConnectionVariable(GetVariableRequestDto connectionVariableRequestDto, CancellationToken cancellationToken = default);
    Task<int> RemoveConnectionCommandList(ConnectionCommandListRequestDto connectionCommandListRequestDto, CancellationToken cancellationToken = default);
    Task<IEnumerable<SetConnectionAccountResponseDto>> SetConnectionAccount(SetConnectionAccountRequestDto request, CancellationToken cancellationToken = default);
    Task<IEnumerable<Guid>> SetConnectionPlayer(SetConnectionPlayerRequestDto request, CancellationToken cancellationToken = default);
    Task<int> UpsertConnectionVariable(UpsertVariableRequestDto upsertVariableRequestDto, CancellationToken cancellationToken = default);
}