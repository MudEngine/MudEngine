CREATE OR ALTER PROCEDURE [Mud].[FindLocalEntity]
	@EntityId int,
	@SearchText varchar(78),
	@Index int
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @Entities TABLE ([RowId] int IDENTITY(1, 1) primary key, [EntityId] int, [Name] varchar(78))
	DECLARE @ParentEntityId int
	SELECT @ParentEntityId=[ParentEntityId] FROM [Mud].[Entity] e WITH (NOLOCK) WHERE [EntityId]=@EntityId AND [InActive]=0
	INSERT INTO @Entities ([EntityId], [Name]) 
	SELECT [EntityId], [Name] FROM [Mud].[Entity] e WITH (NOLOCK) WHERE [ParentEntityId]=@ParentEntityId AND PatIndex('%' + e.[Name] + '%', @SearchText) >  0 AND [InActive]=0
	INSERT INTO @Entities ([EntityId], [Name]) 
	SELECT [EntityId], [Name] FROM [Mud].[Entity] e WITH (NOLOCK) WHERE [ParentEntityId]=@EntityId AND PatIndex('%' + e.[Name] + '%', @SearchText) >  0 AND [InActive]=0
	SELECT [EntityId] FROM @Entities WHERE [RowId]=@Index
END