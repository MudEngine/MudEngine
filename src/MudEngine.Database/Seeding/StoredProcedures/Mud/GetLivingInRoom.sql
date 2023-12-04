CREATE OR ALTER PROCEDURE [Mud].[GetLivingInRoom]
	@RoomId int
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @Entities TABLE ([RowId] int IDENTITY(1, 1) primary key, [EntityId] int INDEX IX_GetEntityDetails_EntityId NONCLUSTERED, [EntityTypeId] int, [Name] varchar(78), [Description] varchar(max), [IsLiving] bit, [ConnectionId] uniqueidentifier null)
	INSERT INTO @Entities ([EntityId], [EntityTypeId], [Name], [IsLiving])
	SELECT e.[EntityId], [EntityTypeId], [Name], 1 AS [IsLiving]
	FROM [Mud].[Entity] e WITH (NOLOCK) 
	INNER JOIN [Mud].[Living] l WITH (NOLOCK) ON e.[EntityId]=l.[EntityId]
	WHERE [ParentEntityId]=@RoomId AND [InActive]=0

	UPDATE ce SET ce.[ConnectionId]=c.[ConnectionId] -- Add Connections
	FROM @Entities ce
	INNER JOIN [Transient].[Connection] c WITH (NOLOCK) ON ce.[EntityId]=c.[PlayerId]

	SELECT [EntityId], [EntityTypeId], [Name], [Description], [IsLiving], [ConnectionId] FROM @Entities
END