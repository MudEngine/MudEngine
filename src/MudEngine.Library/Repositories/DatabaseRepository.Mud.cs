using System.Data;
using System.Diagnostics.CodeAnalysis;
using Dapper;
using Microsoft.Extensions.Logging;
using MudEngine.Library.Domain.Base;
using MudEngine.Library.Domain.Mud;
namespace MudEngine.Library.Repositories;

[SuppressMessage("ReSharper.DPA", "DPA0005: Database issues")]
public partial class DatabaseRepository
{
    public async Task<int> FindLocalEntity(FindLocalEntityRequestDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = connectionFactory.Invoke();
            connection.Open();
            return (await connection.QueryAsync<int?>(
                        new CommandDefinition("[Mud].[FindLocalEntity]",
                            request,
                            commandType: CommandType.StoredProcedure,
                            cancellationToken: cancellationToken))
                    .ConfigureAwait(false))
                .FirstOrDefault() ?? 0;
        }
        catch (Exception e)
        {
            logger.LogError(e, "FindLocalEntity");
            return 0;
        }
    }
    public async Task<GetEntityDetailsResponseDto> GetEntityDetails(int entityId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = connectionFactory.Invoke();
            connection.Open();
            var multi = await connection.QueryMultipleAsync(
                    new CommandDefinition("[Mud].[GetEntityDetails]",
                        new {entityId},
                        commandType: CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
            var environment = (await multi.ReadAsync<GetEntityDetailsResponseDto>()
                .ConfigureAwait(false)).FirstOrDefault() ?? new GetEntityDetailsResponseDto();
            if (environment.EntityId <= 0)
            {
                return environment;
            }
            environment.Entities.AddRange(await multi.ReadAsync<GetEntityDetailsEntityResponseDto>()
                .ConfigureAwait(false));
            return environment;
        }
        catch (Exception e)
        {
            logger.LogError(e, "GetEntityDetails");
            return new GetEntityDetailsResponseDto();
        }
    }
    public async Task<IEnumerable<Entity>> GetLivingInRoom(int roomId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = connectionFactory.Invoke();
            connection.Open();
            return await connection.QueryAsync<Entity>(
                    new CommandDefinition("[Mud].[GetLivingInRoom]",
                        new { roomId },
                        commandType: CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            logger.LogError(e, "GetLivingInRoom");
            return new List<Entity>();
        }
    }
    public async Task<IEnumerable<GetPlayerAliasesResponseDto>> GetPlayerAliases(int playerId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = connectionFactory.Invoke();
            connection.Open();
            return await connection.QueryAsync<GetPlayerAliasesResponseDto>(
                    new CommandDefinition("[Mud].[GetPlayerAliases]",
                        new { playerId },
                        commandType: CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            logger.LogError(e, "GetRoomExits");
            return new List<GetPlayerAliasesResponseDto>();
        }
    }
    public async Task<Player> GetPlayerByName(string playerName, CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = connectionFactory.Invoke();
            connection.Open();
            return (await connection.QueryAsync<Player>(
                    new CommandDefinition("[Mud].[GetPlayerByName]",
                        new { playerName },
                        commandType: CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false)).FirstOrDefault() ?? new Player();
        }
        catch (Exception e)
        {
            logger.LogError(e, "GetEntityDetails");
            return new Player();
        }
    }
    public async Task<IEnumerable<GetRoomExitsResponseDto>> GetRoomExits(int roomId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = connectionFactory.Invoke();
            connection.Open();
            return await connection.QueryAsync<GetRoomExitsResponseDto>(
                    new CommandDefinition("[Mud].[GetRoomExits]",
                        new { roomId },
                        commandType: CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            logger.LogError(e, "GetRoomExits");
            return new List<GetRoomExitsResponseDto>();
        }
    }
    public async Task<int> SetPlayerRoom(SetPlayerRoomRequestDto setPlayerRoomRequestDto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = connectionFactory.Invoke();
            connection.Open();
            var parameters = new DynamicParameters();
            parameters.AddDynamicParams(setPlayerRoomRequestDto);
            parameters.Add("@ReturnCode", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
            _ = await connection.ExecuteScalarAsync(
                    new CommandDefinition("[Mud].[SetPlayerRoom]",
                        parameters,
                        commandType: CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
            return parameters.Get<int>("@ReturnCode");
        }
        catch (Exception e)
        {
            logger.LogError(e, "SetPlayerRoom");
            return -1;
        }
    }
}