CREATE OR ALTER TRIGGER [Mud].[TRG_Entity_OnDelete]
   ON  [Mud].[Entity]
   INSTEAD OF DELETE
AS 
BEGIN
	SET NOCOUNT ON;
	DECLARE @EntityId int
	DECLARE @EntityType varchar(78)
	DECLARE DeleteCursor CURSOR LOCAL FORWARD_ONLY FOR 
		SELECT [EntityId], et.[Name] FROM DELETED d 
			INNER JOIN [Enum].[EntityType] et ON d.[EntityTypeId]=et.[EntityTypeId] 
	OPEN DeleteCursor
	FETCH NEXT FROM DeleteCursor INTO @EntityId, @EntityType
	WHILE @@FETCH_STATUS = 0
	BEGIN
		UPDATE [Mud].[Entity] SET [ParentEntityId]=NULL WHERE [ParentEntityId]=@EntityId
		IF(@EntityType='Mud')
			BEGIN
				DELETE FROM [Mud].[Mud] WHERE [EntityId]=@EntityId
			END
		ELSE IF(@EntityType='Zone')
			BEGIN
				DELETE FROM [Mud].[Zone] WHERE [EntityId]=@EntityId
			END
		ELSE IF(@EntityType='Room')
			BEGIN
				DELETE FROM [Mud].[Room] WHERE [EntityId]=@EntityId
			END
		ELSE IF(@EntityType='Player')
			BEGIN
				DELETE FROM [Mud].[Player] WHERE [EntityId]=@EntityId
				DELETE FROM [Mud].[Living] WHERE [EntityId]=@EntityId
			END
		DELETE [Mud].[Entity] WHERE [EntityId]=@EntityId
		FETCH NEXT FROM DeleteCursor INTO @EntityId, @EntityType
	END
	CLOSE DeleteCursor
	DEALLOCATE DeleteCursor
END