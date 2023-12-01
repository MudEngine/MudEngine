CREATE OR ALTER PROCEDURE [Mud].[GetPlayerAliases]
	@PlayerId int
AS
BEGIN
	SET NOCOUNT ON
	SELECT [Alias], [Replacement] FROM [Mud].[PlayerAlias] WITH (NOLOCK) WHERE [PlayerId]=@PlayerId ORDER BY [Alias]
END