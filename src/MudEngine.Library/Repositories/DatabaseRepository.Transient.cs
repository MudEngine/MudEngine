using System.Data;
using System.Diagnostics.CodeAnalysis;
using Dapper;
using Microsoft.Extensions.Logging;
using MudEngine.Library.Domain.Base;
using MudEngine.Library.Domain.Transient;
namespace MudEngine.Library.Repositories;

[SuppressMessage("ReSharper.DPA", "DPA0005: Database issues")]
public partial class DatabaseRepository
{
    public async Task<int> AddConnectionCommandList(
    ConnectionCommandListRequestDto connectionCommandListRequestDto,
    CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = connectionFactory.Invoke();
            connection.Open();
            var parameters = new DynamicParameters();
            parameters.AddDynamicParams(connectionCommandListRequestDto);
            parameters.Add("@ReturnCode", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
            _ = await connection.ExecuteScalarAsync<Guid>(
                new CommandDefinition("[Transient].[AddConnectionCommandList]",
                    parameters,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);
            return parameters.Get<int>("@ReturnCode");
        }
        catch (Exception e)
        {
            logger.LogError(e, "AddConnectionCommandList");
            return -1;
        }
    }
    public async Task<Player> GetConnectionPlayer(Guid connectionId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = connectionFactory.Invoke();
            connection.Open();
            return (await connection.QueryAsync<Player>(
                        new CommandDefinition("[Transient].[GetConnectionPlayer]",
                            new { connectionId },
                            commandType: CommandType.StoredProcedure,
                            cancellationToken: cancellationToken))
                    .ConfigureAwait(false))
                .FirstOrDefault() ?? new Player();
        }
        catch (Exception e)
        {
            logger.LogError(e, "GetConnectionPlayer");
            return new Player();
        }
    }
    public async Task<string> GetConnectionVariable(GetVariableRequestDto connectionVariableRequestDto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = connectionFactory.Invoke();
            connection.Open();
            return await connection.ExecuteScalarAsync<string>(
                    new CommandDefinition("[Transient].[GetConnectionVariable]",
                        connectionVariableRequestDto,
                        commandType: CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false) ?? string.Empty;
        }
        catch (Exception e)
        {
            logger.LogError(e, "GetConnectionVariable");
            return string.Empty;
        }
    }
    public async Task<int> RemoveConnectionCommandList(
        ConnectionCommandListRequestDto connectionCommandListRequestDto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = connectionFactory.Invoke();
            connection.Open();
            var parameters = new DynamicParameters();
            parameters.AddDynamicParams(connectionCommandListRequestDto);
            parameters.Add("@ReturnCode", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
            _ = await connection.ExecuteScalarAsync<Guid>(
                new CommandDefinition("[Transient].[RemoveConnectionCommandList]",
                    parameters,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);
            return parameters.Get<int>("@ReturnCode");
        }
        catch (Exception e)
        {
            logger.LogError(e, "RemoveConnectionCommandList");
            return -1;
        }
    }
    public async Task<IEnumerable<SetConnectionAccountResponseDto>> SetConnectionAccount(
        SetConnectionAccountRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = connectionFactory.Invoke();
            connection.Open();
            return await connection.QueryAsync<SetConnectionAccountResponseDto>(
                    new CommandDefinition("[Transient].[SetConnectionAccount]",
                        request,
                        commandType: CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            logger.LogError(e, "SetConnectionAccount");
            return new List<SetConnectionAccountResponseDto>();
        }
    }
    public async Task<IEnumerable<Guid>> SetConnectionPlayer(
        SetConnectionPlayerRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = connectionFactory.Invoke();
            connection.Open();
            return await connection.QueryAsync<Guid>(
                    new CommandDefinition("[Transient].[SetConnectionPlayer]",
                        request,
                        commandType: CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            logger.LogError(e, "SetConnectionPlayer");
            return new List<Guid>();
        }
    }
    public async Task<int> UpsertConnectionVariable(UpsertVariableRequestDto upsertVariableRequestDto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = connectionFactory.Invoke();
            connection.Open();
            var parameters = new DynamicParameters();
            parameters.AddDynamicParams(upsertVariableRequestDto);
            parameters.Add("@ReturnCode", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
            _ = await connection.ExecuteScalarAsync<Guid>(
                new CommandDefinition("[Transient].[UpsertConnectionVariable]",
                    parameters,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);
            return parameters.Get<int>("@ReturnCode");
        }
        catch (Exception e)
        {
            logger.LogError(e, "UpsertConnectionVariable");
            return -1;
        }
    }
}