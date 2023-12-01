CREATE OR ALTER PROCEDURE [System].[GetMSSP]
	@Uptime varchar(78)
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @MSSP TABLE([Key] varchar(78), [Value] varchar(max))
	INSERT INTO @MSSP ([Key], [Value])
	SELECT TOP 1 'NAME' AS [Key], e.[Name] AS [Value]  
	FROM [Enum].[EntityType] et WITH (NOLOCK)
	INNER JOIN [Mud].[Entity] e WITH (NOLOCK) ON et.[EntityTypeId]=e.[EntityTypeId]
	WHERE et.[Name] = 'Mud'
	INSERT INTO @MSSP ([Key], [Value]) VALUES('PLAYERS', '-1')
	INSERT INTO @MSSP ([Key], [Value]) VALUES('UPTIME', @Uptime)
	SELECT [Key], [Value] FROM @MSSP
END