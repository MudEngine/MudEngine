CREATE OR ALTER PROCEDURE [System].[OnNewConnection]
	@ConnectionId UniqueIdentifier,
	@AdditionalData varchar(max)
AS
BEGIN
	SET NOCOUNT ON
	BEGIN TRY
		DECLARE @Now datetime2(7)=getdate()
		INSERT INTO [Transient].[Connection] ([ConnectionId], [CreatedOn], [LastCommandRequestedOn]) VALUES (@ConnectionId, @Now, @Now)
		SELECT TOP 1 m.[OnNewConnectionCommandId] FROM [Mud].[Mud] m WITH (NOLOCK) 
			INNER JOIN [Mud].[Entity] e WITH (NOLOCK) ON m.[EntityId]=e.[EntityId] 
			ORDER BY e.[CreatedOn] DESC
		RETURN 0
	END TRY
	BEGIN CATCH
		RETURN -1
	END CATCH	
END