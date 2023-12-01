CREATE OR ALTER PROCEDURE [System].[OnStartup]
AS
BEGIN
	SET NOCOUNT ON
	BEGIN TRY
		DELETE FROM [Transient].[ConnectionVariable]
		DELETE FROM [Transient].[ConnectionCommand]
		DELETE FROM [Transient].[Connection]
		DECLARE @EntityTypeId int
		SELECT @EntityTypeId=[EntityTypeId] FROM [Enum].[EntityType] WHERE [Name]='Player'
		UPDATE [Mud].[Entity] SET [InActive]=1 WHERE [EntityTypeId]=@EntityTypeId
		RETURN 0
	END TRY
	BEGIN CATCH
		RETURN -1
	END CATCH	
END