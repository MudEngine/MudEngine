CREATE OR ALTER PROCEDURE [System].[UpsertRoom]
	@ZoneId int,
	@Name varchar(78),
	@Description varchar(max)
AS
BEGIN
	SET NOCOUNT ON
	SET XACT_ABORT ON
	BEGIN TRY
		DECLARE @EntityId int
		BEGIN TRANSACTION
			DECLARE @Now datetime2(7)=getdate()
			DECLARE @EntityTypeId int
			SELECT @EntityTypeId=[EntityTypeId] FROM [Enum].[EntityType] WHERE [Name]='Room'
			DECLARE @EntityIds TABLE (EntityId int)
			UPDATE [Mud].[Entity] WITH (UPDLOCK, SERIALIZABLE) SET [LastUpdatedOn]=@Now, @EntityId=[EntityId], [Description]=@Description, [ParentEntityId]=@ZoneId WHERE [EntityTypeId]=@EntityTypeId AND [ParentEntityId]=@ZoneId AND [Name]=@Name
			IF (@@ROWCOUNT = 0)
			BEGIN
			  INSERT [Mud].[Entity]([EntityTypeId], [Name], [Description], [ParentEntityId], [CreatedOn], [LastUpdatedOn]) OUTPUT inserted.[EntityId] INTO @EntityIds VALUES (@EntityTypeId, @Name, @Description, @ZoneId, @Now, @Now)
			  SELECT @EntityId=[EntityId] FROM @EntityIds
			  INSERT [Mud].[Room]([EntityId]) VALUES (@EntityId)
			END
		COMMIT
		RETURN @EntityId
	END TRY
	BEGIN CATCH
		ROLLBACK
		RETURN -1
	END CATCH	
END