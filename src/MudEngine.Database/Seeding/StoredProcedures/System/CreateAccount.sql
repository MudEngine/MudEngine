CREATE OR ALTER PROCEDURE [System].[CreateAccount]
	@Name varchar(78),
	@HashedPassword varchar(max)
AS
BEGIN
	SET NOCOUNT ON
	SET XACT_ABORT ON
	BEGIN TRY
		DECLARE @ReturnCode int
		BEGIN TRANSACTION
			IF EXISTS (SELECT 1 FROM [System].[Account] WHERE [Name]=@Name)
				BEGIN
					SET @ReturnCode = -2
				END
			ELSE
				BEGIN
					IF EXISTS (SELECT 1 FROM [Mud].[Entity] WHERE [Name]=@Name)
						BEGIN
							SET @ReturnCode = -3
						END
					ELSE
						BEGIN
							DECLARE @AccountId uniqueidentifier
							DECLARE @AccountIds TABLE (AccountId uniqueidentifier)
							DECLARE @Now datetime2(7)=getdate()
							INSERT [System].[Account]([Name], [HashedPassword], [CreatedOn], [LastAccessed]) OUTPUT inserted.[AccountId] INTO @AccountIds  VALUES (@Name, @HashedPassword, @Now, @Now)
							SELECT @AccountId=[AccountId] FROM @AccountIds
							SELECT @AccountId AS [AccountId]
							SET @ReturnCode = 0
						END
				END
		COMMIT
		RETURN @ReturnCode
	END TRY
	BEGIN CATCH
		ROLLBACK
		RETURN -1
	END CATCH	
END