CREATE OR ALTER PROCEDURE [Transient].[UpsertConnectionVariable]
	@ConnectionId UniqueIdentifier,
	@Name varchar(78),
	@Value varchar(max)
AS
BEGIN
	SET NOCOUNT ON
	SET XACT_ABORT ON
	BEGIN TRY
		BEGIN TRANSACTION
			UPDATE [Transient].[ConnectionVariable] WITH (SERIALIZABLE) SET [Value]=@Value WHERE [ConnectionId]=@ConnectionId AND [Name]=@Name
			IF (@@ROWCOUNT = 0)
			BEGIN
				INSERT [Transient].[ConnectionVariable]([ConnectionId], [Name], [Value]) VALUES (@ConnectionId, @Name, @Value)
			END
		COMMIT
		RETURN 0
	END TRY
	BEGIN CATCH
		ROLLBACK
		RETURN -1
	END CATCH	
END