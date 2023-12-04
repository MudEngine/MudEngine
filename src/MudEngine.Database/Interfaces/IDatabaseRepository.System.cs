using MudEngine.Database.DataTransferObjects.System;
namespace MudEngine.Database.Interfaces;

public partial interface IDatabaseRepository
{
    Task<GetAccountForLoginResponseDto> GetAccountForLogin(string accountName, CancellationToken cancellationToken = default);
    Task<Guid> GetCommandByListAndAlias(GetCommandByListAndAliasRequestDto commandRequestDto, CancellationToken cancellationToken = default);
    Task<IEnumerable<GetMsspResponseDto>> GetMssp(string uptime, CancellationToken cancellationToken = default);
    Task<GetMudByNameResponseDto> GetMudByName(string mudName, CancellationToken cancellationToken = default);
    Task<int> OnDisconnect(Guid connectionId, CancellationToken cancellationToken = default);
    Task<Guid> OnNewConnection(OnNewConnectionRequestDto onNewConnectionRequestDto, CancellationToken cancellationToken = default);
    Task<int> OnStartup(CancellationToken cancellationToken = default);
    Task<Guid> OnUserCommand(CommandRequestDto commandRequestDto, CancellationToken cancellationToken = default);
}