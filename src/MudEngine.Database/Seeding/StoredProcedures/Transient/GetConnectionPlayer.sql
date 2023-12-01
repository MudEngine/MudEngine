CREATE OR ALTER PROCEDURE [Transient].[GetConnectionPlayer]
	@ConnectionId UniqueIdentifier
AS
BEGIN
	SET NOCOUNT ON
	SELECT TOP 1 e.[EntityId], e.[Name], e.[ParentEntityId] AS [RoomId]
	FROM [Transient].[Connection] c WITH (NOLOCK) 
	INNER JOIN [Mud].[Entity] e WITH (NOLOCK) ON c.[PlayerId]=e.[EntityId]
	WHERE [ConnectionId]=@ConnectionId
END