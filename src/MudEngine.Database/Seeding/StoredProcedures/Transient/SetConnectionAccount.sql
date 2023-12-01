CREATE OR ALTER PROCEDURE [Transient].[SetConnectionAccount]
	@ConnectionId UniqueIdentifier,
	@AccountId UniqueIdentifier
AS
BEGIN
	SET NOCOUNT ON
	BEGIN TRY
		DECLARE @Now datetime2(7)=getdate()
		UPDATE [System].[Account] WITH (UPDLOCK, SERIALIZABLE) SET [LastAccessed]=@Now WHERE [AccountId]=@AccountId
		UPDATE [Transient].[Connection] WITH (SERIALIZABLE) SET [AccountId]=@AccountId WHERE [ConnectionId]=@ConnectionId
		SELECT p.[EntityId], e.[Name] 
		FROM [Mud].[Player] p WITH (NOLOCK) 
		INNER JOIN [Mud].[Entity] e WITH (NOLOCK) ON p.[EntityId]=e.[EntityId]
		WHERE p.[AccountId]=@AccountId
		RETURN 0
	END TRY
	BEGIN CATCH
		RETURN -1
	END CATCH	
END