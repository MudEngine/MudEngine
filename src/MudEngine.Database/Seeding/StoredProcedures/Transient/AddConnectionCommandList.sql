CREATE OR ALTER PROCEDURE [Transient].[AddConnectionCommandList]
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
				INSERT INTO [Transient].[ConnectionCommand] ([ConnectionId], [CommandListId]) VALUES (@ConnectionId, @CommandListId)
				SET @ReturnCode = 0
			END
		RETURN @ReturnCode
	END TRY
	BEGIN CATCH
		RETURN -1
	END CATCH	
END