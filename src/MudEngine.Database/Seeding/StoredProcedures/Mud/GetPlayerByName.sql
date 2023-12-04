CREATE OR ALTER PROCEDURE [Mud].[GetPlayerByName]
	@PlayerName varchar(78)
AS
BEGIN
	SET NOCOUNT ON
	SELECT e.[EntityId], [EntityTypeId], [Name], [Description], 1 AS [IsLiving], e.[ParentEntityId] AS [RoomId], [ConnectionId]
	FROM [Mud].[Entity] e WITH (NOLOCK) 
	INNER JOIN [Mud].[Player] p WITH (NOLOCK) ON e.[EntityId]=p.[EntityId]
	INNER JOIN [Transient].[Connection] c WITH (NOLOCK) ON e.[EntityId]=c.[PlayerId]
	WHERE e.[Name]=@PlayerName AND [InActive]=0
END