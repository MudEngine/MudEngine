CREATE OR ALTER PROCEDURE [System].[UpsertCommandAssembly]
	@CommandAssemblyId uniqueidentifier,
	@Preload bit,
	@SourceCode varchar(max)=null,
	@Binary varbinary(max)=null
AS
BEGIN
	SET NOCOUNT ON
	SET XACT_ABORT ON
	BEGIN TRY
		BEGIN TRANSACTION
			UPDATE [System].[CommandAssembly] WITH (UPDLOCK, SERIALIZABLE) SET [Preload]=@Preload, [SourceCode]=@SourceCode, [Binary]=@Binary WHERE [CommandAssemblyId]=@CommandAssemblyId
			IF (@@ROWCOUNT = 0)
			BEGIN
			  INSERT [System].[CommandAssembly]([CommandAssemblyId], [Preload], [SourceCode], [Binary]) VALUES (@CommandAssemblyId, @Preload, @SourceCode, @Binary)
			END
		COMMIT
		RETURN 0
	END TRY
	BEGIN CATCH
		ROLLBACK
		RETURN -1
	END CATCH	
END