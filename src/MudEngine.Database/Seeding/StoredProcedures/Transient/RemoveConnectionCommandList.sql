CREATE OR ALTER PROCEDURE [Transient].[RemoveConnectionCommandList]
	@ConnectionId UniqueIdentifier,
	@CommandListName varchar(78)
AS
BEGIN
	SET NOCOUNT ON
	BEGIN TRY
		DECLARE @ReturnCode int
		DECLARE @CommandListId int
		SELECT @CommandListId=[CommandListId] FROM [System].[CommandList] WITH (NOLOCK) WHERE [Name]=@CommandListName
		IF ISNULL(@CommandListId, 0) = 0
			BEGIN
				SET @ReturnCode = -2
			END
		ELSE
			BEGIN
				DELETE FROM [Transient].[ConnectionCommand] WHERE [ConnectionId]=@ConnectionId AND [CommandListId]=@CommandListId
				SET @ReturnCode = 0
			END
		RETURN @ReturnCode
	END TRY
	BEGIN CATCH
		RETURN -1
	END CATCH	
END