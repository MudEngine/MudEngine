CREATE OR ALTER PROCEDURE [Mud].[SetPlayerRoom]
	@PlayerId int,
	@DestinationRoomId int
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @ReturnCode int
	BEGIN TRY
		UPDATE [Mud].[Entity] WITH (UPDLOCK, SERIALIZABLE) SET [ParentEntityId]=@DestinationRoomId WHERE [EntityId]=@PlayerId
		IF (@@ROWCOUNT = 0)
			BEGIN
				SET @ReturnCode=-2
			END
		ELSE
			BEGIN
				SET @ReturnCode=0
			END
		RETURN @ReturnCode
	END TRY
	BEGIN CATCH
		RETURN -1
	END CATCH
END