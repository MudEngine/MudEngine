CREATE OR ALTER PROCEDURE [System].[GetCommandByListAndAlias]
	@CommandListName varchar(78),
	@PrimaryAlias varchar(78)
AS
BEGIN
	SET NOCOUNT ON
	SELECT cla.[CommandAssemblyId] 
	FROM [System].[CommandList] cl WITH (NOLOCK)
	INNER JOIN [System].[CommandListAssembly] cla WITH (NOLOCK) ON cl.[CommandListId] =cla.[CommandListId]
	WHERE cl.[Name]=@CommandListName AND cla.[PrimaryAlias]=@PrimaryAlias
END