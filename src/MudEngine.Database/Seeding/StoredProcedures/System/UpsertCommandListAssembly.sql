CREATE OR ALTER PROCEDURE [System].[UpsertCommandListAssembly]
	@Name varchar(78),
	@CommandAssemblyId uniqueidentifier,
	@PrimaryAlias varchar(78),
	@HandlesUnknown bit = 0
AS
BEGIN
	SET NOCOUNT ON
	SET XACT_ABORT ON
	BEGIN TRY
		DECLARE @ReturnCode int
		BEGIN TRANSACTION
			DECLARE @CommandListId int
			SELECT @CommandListId=[CommandListId] FROM [System].[CommandList] WITH (NOLOCK) WHERE [Name]=@Name
			IF ISNULL(@CommandListId, 0) = 0
				BEGIN
					SET @ReturnCode = -2
				END
			ELSE
				BEGIN
					UPDATE [System].[CommandListAssembly] WITH (UPDLOCK, SERIALIZABLE) SET [PrimaryAlias]=@PrimaryAlias, [HandlesUnknown]=@HandlesUnknown WHERE [CommandListId]=@CommandListId AND [CommandAssemblyId]=@CommandAssemblyId
					IF (@@ROWCOUNT = 0)
					BEGIN
					  INSERT [System].[CommandListAssembly]([CommandListId], [CommandAssemblyId], [PrimaryAlias], [HandlesUnknown]) VALUES (@CommandListId, @CommandAssemblyId, @PrimaryAlias, @HandlesUnknown)
					END
					SET @ReturnCode = 0
				END
		COMMIT
		RETURN @ReturnCode
	END TRY
	BEGIN CATCH
		ROLLBACK
		RETURN -1
	END CATCH	
END