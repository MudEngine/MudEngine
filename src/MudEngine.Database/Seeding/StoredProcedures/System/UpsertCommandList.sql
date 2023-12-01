CREATE OR ALTER PROCEDURE [System].[UpsertCommandList]
	@Name varchar(78),
	@Priority int
AS
BEGIN
	SET NOCOUNT ON
	SET XACT_ABORT ON
	BEGIN TRY
		DECLARE @CommandListId int
		BEGIN TRANSACTION
			UPDATE [System].[CommandList] WITH (UPDLOCK, SERIALIZABLE) SET [Priority]=@Priority WHERE [Name]=@Name
			IF (@@ROWCOUNT = 0)
			BEGIN
			  INSERT [System].[CommandList]([Name], [Priority]) VALUES (@Name, @Priority)
			END
		COMMIT
		RETURN 0
	END TRY
	BEGIN CATCH
		ROLLBACK
		RETURN -1
	END CATCH	
END