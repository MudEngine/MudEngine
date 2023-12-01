CREATE OR ALTER PROCEDURE [Transient].[SetConnectionPlayer]
	@ConnectionId uniqueidentifier,
	@PlayerId int
AS
BEGIN
	SET NOCOUNT ON
	BEGIN TRY
		DECLARE @AccountId UniqueIdentifier
		SELECT @AccountId=[AccountId] FROM [Transient].[Connection] WITH (NOLOCK) WHERE [ConnectionId]=@ConnectionId
		UPDATE [Transient].[Connection] WITH (SERIALIZABLE) SET [PlayerId]=@PlayerId WHERE [ConnectionId]=@ConnectionId
		DECLARE @ConnectionIds TABLE (ConnectionId uniqueidentifier)
		INSERT INTO @ConnectionIds (ConnectionId) SELECT [ConnectionId] FROM [Transient].[Connection] WITH (NOLOCK) WHERE [AccountId]=@AccountId AND [ConnectionId]!=@ConnectionId
		IF EXISTS(SELECT 1 FROM @ConnectionIds)
		BEGIN
			DECLARE @ConnectionIdToClose uniqueidentifier
			DECLARE CloseConnectionCursor CURSOR LOCAL FOR SELECT [ConnectionId] FROM @ConnectionIds
			OPEN CloseConnectionCursor
			FETCH NEXT FROM CloseConnectionCursor INTO @ConnectionIdToClose
			WHILE @@FETCH_STATUS = 0 
				BEGIN
					EXEC [System].[OnDisconnect] @ConnectionIdToClose
					FETCH NEXT FROM CloseConnectionCursor INTO @ConnectionIdToClose
				END
			CLOSE CloseConnectionCursor
			DEALLOCATE CloseConnectionCursor
		END
		UPDATE [Mud].[Entity] WITH (UPDLOCK, SERIALIZABLE) SET [InActive]=0 WHERE [EntityId]=@PlayerId
		SELECT [ConnectionId] FROM @ConnectionIds
		RETURN 0
	END TRY
	BEGIN CATCH
		RETURN -1
	END CATCH
END