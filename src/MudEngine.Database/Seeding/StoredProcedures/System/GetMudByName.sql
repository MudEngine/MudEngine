CREATE OR ALTER PROCEDURE [System].[GetMudByName]
	@MudName varchar(78)
AS
BEGIN
	SET NOCOUNT ON
	IF LEN(ISNULL(@MudName, '')) > 0
		BEGIN
			SELECT m.[EntityId], e.[Name], m.[LoginScreen], m.[News] FROM [Mud].[Mud] m WITH (NOLOCK)
			INNER JOIN [Mud].[Entity] e WITH (NOLOCK) ON m.[EntityId]=e.[EntityId]
			WHERE e.[Name]=@MudName
		END
	ELSE
		BEGIN
			SELECT TOP 1 m.[EntityId], e.[Name], m.[LoginScreen], m.[News] FROM [Mud].[Mud] m WITH (NOLOCK)
			INNER JOIN [Mud].[Entity] e WITH (NOLOCK) ON m.[EntityId]=e.[EntityId]
			ORDER BY e.[CreatedOn] DESC
		END
END