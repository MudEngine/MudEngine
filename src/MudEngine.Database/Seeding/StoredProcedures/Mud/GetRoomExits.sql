CREATE OR ALTER PROCEDURE [Mud].[GetRoomExits]
	@RoomId int
AS
BEGIN
	SET NOCOUNT ON
	SELECT [SourceId],[DestinationId],[PrimaryAlias],[RoomExitVisibilityId]
	FROM [Mud].[RoomExit] WITH (NOLOCK)
	WHERE [SourceId]=@RoomId
	ORDER BY [PrimaryAlias]
END