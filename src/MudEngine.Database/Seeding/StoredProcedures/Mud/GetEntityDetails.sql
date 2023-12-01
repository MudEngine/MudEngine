CREATE OR ALTER PROCEDURE [Mud].[GetEntityDetails]
	@EntityId int
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @Entities TABLE ([RowId] int IDENTITY(1, 1) primary key, [EntityId] int INDEX IX_GetEntityDetails_EntityId NONCLUSTERED, [EntityTypeId] int, [Name] varchar(78), [Description] varchar(max), [IsLiving] bit, [ConnectionId] uniqueidentifier null)

	INSERT INTO @Entities ([EntityId], [EntityTypeId], [Name], [Description], [IsLiving]) -- Entity
		SELECT e.[EntityId], [EntityTypeId], [Name], [Description], CASE WHEN l.[EntityId] Is Null THEN 0 ELSE 1 END AS [IsLiving]
		FROM [Mud].[Entity] e WITH (NOLOCK) 
		LEFT JOIN [Mud].[Living] l WITH (NOLOCK) ON e.[EntityId]=l.[EntityId]
		WHERE e.[EntityId]=@EntityId AND [InActive]=0

	INSERT INTO @Entities ([EntityId], [EntityTypeId], [Name], [IsLiving]) -- Child Entities
		SELECT e.[EntityId], [EntityTypeId], [Name], CASE WHEN l.[EntityId] Is Null THEN 0 ELSE 1 END AS [IsLiving]
		FROM [Mud].[Entity] e WITH (NOLOCK) 
		LEFT JOIN [Mud].[Living] l WITH (NOLOCK) ON e.[EntityId]=l.[EntityId]
		WHERE [ParentEntityId]=@EntityId AND [InActive]=0

	UPDATE ce SET ce.[ConnectionId]=c.[ConnectionId] -- Add Connections
	FROM @Entities ce
	INNER JOIN [Transient].[Connection] c WITH (NOLOCK) ON ce.[EntityId]=c.[PlayerId]

	SELECT [EntityId], [EntityTypeId], [Name], [Description], [IsLiving], [ConnectionId] FROM @Entities WHERE [RowId] = 1

	SELECT [EntityId], [EntityTypeId], [Name], [IsLiving], [ConnectionId] FROM @Entities WHERE [RowId] > 1
END