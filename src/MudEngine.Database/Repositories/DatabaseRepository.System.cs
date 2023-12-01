using System.Data;
using System.Diagnostics.CodeAnalysis;
using Dapper;
using MudEngine.Database.DataTransferObjects.System;
namespace MudEngine.Database.Repositories;

[SuppressMessage("ReSharper.DPA", "DPA0005: Database issues")]
public partial class DatabaseRepository
{
    public async Task<GetAccountForLoginResponseDto> GetAccountForLogin(string accountName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = connectionFactory.Invoke();
            connection.Open();
            return (await connection.QueryAsync<GetAccountForLoginResponseDto>(
                        new CommandDefinition("[System].[GetAccountForLogin]",
                            new {accountName},
                            commandType: CommandType.StoredProcedure,
                            cancellationToken: cancellationToken))
                    .ConfigureAwait(false))
                .FirstOrDefault() ?? new GetAccountForLoginResponseDto();
        }
        catch (Exception e)
        {
            logger.LogError(e, "GetAccountForLogin");
            return new GetAccountForLoginResponseDto();
        }
    }
    public async Task<Guid> GetCommandByListAndAlias(GetCommandByListAndAliasRequestDto commandRequestDto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = connectionFactory.Invoke();
            connection.Open();
            return await connection.ExecuteScalarAsync<Guid>(
                    new CommandDefinition("[System].[GetCommandByListAndAlias]",
                        commandRequestDto,
                        commandType: CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            logger.LogError(e, "GetCommandByListAndAlias");
            return Guid.Empty;
        }
    }
    public async Task<IEnumerable<GetMsspResponseDto>> GetMssp(string uptime,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = connectionFactory.Invoke();
            connection.Open();
            return await connection.QueryAsync<GetMsspResponseDto>(
                    new CommandDefinition("[System].[GetMSSP]",
                        new {uptime},
                        commandType: CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            logger.LogError(e, "GetMssp");
            return new List<GetMsspResponseDto>();
        }
    }
    public async Task<GetMudByNameResponseDto> GetMudByName(string mudName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = connectionFactory.Invoke();
            connection.Open();
            return (await connection.QueryAsync<GetMudByNameResponseDto>(
                        new CommandDefinition("[System].[GetMudByName]",
                            new {mudName},
                            commandType: CommandType.StoredProcedure,
                            cancellationToken: cancellationToken))
                    .ConfigureAwait(false))
                .FirstOrDefault() ?? new GetMudByNameResponseDto();
        }
        catch (Exception e)
        {
            logger.LogError(e, "GetMudByName");
            return new GetMudByNameResponseDto();
        }
    }
    public async Task<int> OnDisconnect(Guid connectionId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = connectionFactory.Invoke();
            connection.Open();
            var parameters = new DynamicParameters();
            parameters.Add("@ConnectionId", connectionId, DbType.Guid, ParameterDirection.Input);
            parameters.Add("@ReturnCode", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
            _ = await connection.ExecuteScalarAsync(
                    new CommandDefinition("[System].[OnDisconnect]",
                        parameters,
                        commandType: CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
            return parameters.Get<int>("@ReturnCode");
        }
        catch (Exception e)
        {
            logger.LogError(e, "OnDisconnect");
            return -1;
        }
    }
    public async Task<Guid> OnNewConnection(OnNewConnectionRequestDto onNewConnectionRequestDto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = connectionFactory.Invoke();
            connection.Open();
            return await connection.ExecuteScalarAsync<Guid>(
                    new CommandDefinition("[System].[OnNewConnection]",
                        onNewConnectionRequestDto,
                        commandType: CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            logger.LogError(e, "NewConnection");
            return Guid.Empty;
        }
    }
    public async Task<Guid> OnUserCommand(CommandRequestDto commandRequestDto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = connectionFactory.Invoke();
            connection.Open();
            return await connection.ExecuteScalarAsync<Guid>(
                    new CommandDefinition("[System].[OnUserCommand]",
                        commandRequestDto,
                        commandType: CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            logger.LogError(e, "OnUserCommand");
            return Guid.Empty;
        }
    }
}