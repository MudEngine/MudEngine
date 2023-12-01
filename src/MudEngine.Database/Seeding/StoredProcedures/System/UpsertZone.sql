CREATE OR ALTER PROCEDURE [System].[UpsertZone]
	@MudId int,
	@Name varchar(78)
AS
BEGIN
	SET NOCOUNT ON
	SET XACT_ABORT ON
	BEGIN TRY
		DECLARE @EntityId int
		BEGIN TRANSACTION
			DECLARE @Now datetime2(7)=getdate()
			DECLARE @EntityTypeId int
			SELECT @EntityTypeId=[EntityTypeId] FROM [Enum].[EntityType] WHERE [Name]='Zone'
			DECLARE @EntityIds TABLE (EntityId int)
			UPDATE [Mud].[Entity] WITH (UPDLOCK, SERIALIZABLE) SET [LastUpdatedOn]=@Now, @EntityId=[EntityId], [ParentEntityId]=@MudId WHERE [EntityTypeId]=@EntityTypeId AND [Name]=@Name
			IF (@@ROWCOUNT = 0)
			BEGIN
			  INSERT [Mud].[Entity]([EntityTypeId], [Name], [ParentEntityId], [CreatedOn], [LastUpdatedOn]) OUTPUT inserted.[EntityId] INTO @EntityIds VALUES (@EntityTypeId, @Name, @MudId, @Now, @Now)
			  SELECT @EntityId=[EntityId] FROM @EntityIds
			  INSERT [Mud].[Zone]([EntityId]) VALUES (@EntityId)
			END
		COMMIT
		RETURN @EntityId
	END TRY
	BEGIN CATCH
		ROLLBACK
		RETURN -1
	END CATCH	
END