CREATE OR ALTER PROCEDURE [System].[UpsertMud]
	@Name varchar(78),
	@OnNewConnectionCommandId uniqueidentifier
AS
BEGIN
	SET NOCOUNT ON
	SET XACT_ABORT ON
	BEGIN TRY
		DECLARE @EntityId int
		BEGIN TRANSACTION
			DECLARE @Now datetime2(7)=getdate()
			DECLARE @EntityTypeId int
			SELECT @EntityTypeId=[EntityTypeId] FROM [Enum].[EntityType] WHERE [Name]='Mud'
			DECLARE @EntityIds TABLE (EntityId int)
			UPDATE [Mud].[Entity] WITH (UPDLOCK, SERIALIZABLE) SET [LastUpdatedOn]=@Now, @EntityId=[EntityId] WHERE [EntityTypeId]=@EntityTypeId AND [Name]=@Name
			IF (@@ROWCOUNT = 0)
			BEGIN
			  INSERT [Mud].[Entity]([EntityTypeId], [Name], [CreatedOn], [LastUpdatedOn]) OUTPUT inserted.[EntityId] INTO @EntityIds VALUES (@EntityTypeId, @Name, @Now, @Now)
			  SELECT @EntityId=[EntityId] FROM @EntityIds
			END
			UPDATE [Mud].[Mud] WITH (UPDLOCK, SERIALIZABLE) SET [OnNewConnectionCommandId]=@OnNewConnectionCommandId WHERE [EntityId]=@EntityId
			IF (@@ROWCOUNT = 0)
				BEGIN
					INSERT [Mud].[Mud]([EntityId], [OnNewConnectionCommandId]) VALUES (@EntityId, @OnNewConnectionCommandId)
				END
		COMMIT
		RETURN @EntityId
	END TRY
	BEGIN CATCH
		ROLLBACK
		RETURN -1
	END CATCH	
END