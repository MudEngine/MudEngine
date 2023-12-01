CREATE OR ALTER PROCEDURE [System].[OnUserCommand]
	@ConnectionId UniqueIdentifier,
	@CommandLine varchar(max)
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @Now datetime2(7)=getdate()
	UPDATE [Transient].[Connection] SET [LastCommandRequestedOn]=@Now
	IF (@@ROWCOUNT = 0)
		BEGIN
			SELECT cl.[Priority], cla.* 
			FROM [System].[CommandList] cl
			INNER JOIN [System].[CommandListAssembly] cla WITH (NOLOCK) ON cl.[CommandListId] =cla.[CommandListId]
			WHERE cl.[Name]='New Connections' AND cla.[PrimaryAlias]='quit'
		END
	ELSE
		BEGIN
			SELECT TOP 1 j.[CommandAssemblyId] FROM (
				SELECT cl.[Priority], cla.* 
				FROM [Transient].[ConnectionCommand] c WITH (NOLOCK)
				INNER JOIN [System].[CommandList] cl WITH (NOLOCK) ON c.[CommandListId] =cl.[CommandListId] 
				INNER JOIN [System].[CommandListAssembly] cla WITH (NOLOCK) ON cl.[CommandListId] =cla.[CommandListId]
				WHERE c.[ConnectionId]=@ConnectionId AND PatIndex(cla.[PrimaryAlias] + '%', @CommandLine) > 0
			UNION
				SELECT cl.[Priority], cla.* 
				FROM [Transient].[ConnectionCommand] c WITH (NOLOCK)
				INNER JOIN [System].[CommandList] cl WITH (NOLOCK) ON c.[CommandListId] =cl.[CommandListId] 
				INNER JOIN [System].[CommandListAssembly] cla WITH (NOLOCK) ON cl.[CommandListId] =cla.[CommandListId]
				WHERE c.[ConnectionId]=@ConnectionId AND cla.[HandlesUnknown]=1
			) j ORDER BY [HandlesUnknown], [Priority]
	END
END