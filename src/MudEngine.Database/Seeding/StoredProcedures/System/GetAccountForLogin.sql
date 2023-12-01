CREATE OR ALTER PROCEDURE [System].[GetAccountForLogin]
	@AccountName varchar(78)
AS
BEGIN
	SET NOCOUNT ON
	SELECT [AccountId], [HashedPassword] FROM [System].[Account] WITH (NOLOCK) WHERE [Name]=@AccountName
END