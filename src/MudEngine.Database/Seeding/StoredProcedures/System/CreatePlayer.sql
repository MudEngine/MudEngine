CREATE OR ALTER PROCEDURE [System].[CreatePlayer]
	@AccountId uniqueidentifier,
	@RoomId int,
	@Name varchar(78)
AS
BEGIN
	SET NOCOUNT ON
	SET XACT_ABORT ON
	BEGIN TRY
		DECLARE @ReturnCode int
		BEGIN TRANSACTION
			DECLARE @EntityTypeId int
			SELECT @EntityTypeId=[EntityTypeId] FROM [Enum].[EntityType] WHERE [Name]='Player'
			IF EXISTS (SELECT 1 FROM [Mud].[Entity] WHERE [EntityTypeId]=@EntityTypeId AND [Name]=@Name)
				BEGIN
					SET @ReturnCode = -2
				END
			ELSE
				BEGIN
					DECLARE @EntityId int
					DECLARE @EntityIds TABLE (EntityId int)
					DECLARE @Now datetime2(7)=getdate()
					INSERT [Mud].[Entity]([EntityTypeId], [Name], [ParentEntityId], [Inactive], [CreatedOn], [LastUpdatedOn]) OUTPUT inserted.[EntityId] INTO @EntityIds VALUES (@EntityTypeId, @Name, @RoomId, 1, @Now, @Now)
					SELECT @EntityId=[EntityId] FROM @EntityIds
					INSERT [Mud].[Living]([EntityId]) VALUES (@EntityId)
					INSERT [Mud].[Player]([EntityId], [AccountId]) VALUES (@EntityId, @AccountId)
					SELECT @EntityId AS [EntityId]
					SET @ReturnCode = @EntityId
				END
		COMMIT
		RETURN @ReturnCode
	END TRY
	BEGIN CATCH
		ROLLBACK
		RETURN -1
	END CATCH	
END