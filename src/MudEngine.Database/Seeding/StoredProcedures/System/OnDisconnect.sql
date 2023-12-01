CREATE OR ALTER PROCEDURE [System].[OnDisconnect]
	@ConnectionId UniqueIdentifier
AS
BEGIN
	SET NOCOUNT ON
	SET XACT_ABORT ON
	BEGIN TRY
		BEGIN TRANSACTION
			DECLARE @PlayerId int
			SELECT @PlayerId=[PlayerId] FROM [Transient].[Connection] WITH (NOLOCK) WHERE [ConnectionId]=@ConnectionId
			DELETE FROM [Transient].[ConnectionVariable] WHERE [ConnectionId]=@ConnectionId
			DELETE FROM [Transient].[ConnectionCommand] WHERE [ConnectionId]=@ConnectionId
			DELETE FROM [Transient].[Connection] WHERE [ConnectionId]=@ConnectionId
			IF ISNULL(@PlayerId, 0) > 0
				BEGIN
					UPDATE [Mud].[Entity] SET [InActive]=1 WHERE [EntityId]=@PlayerId
				END
		COMMIT
		RETURN 0
	END TRY
	BEGIN CATCH
		ROLLBACK
		RETURN -1
	END CATCH	
END