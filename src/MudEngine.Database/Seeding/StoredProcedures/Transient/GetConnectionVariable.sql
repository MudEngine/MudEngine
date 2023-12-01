CREATE OR ALTER PROCEDURE [Transient].[GetConnectionVariable]
	@ConnectionId UniqueIdentifier,
	@Name varchar(78)
AS
BEGIN
	SET NOCOUNT ON
	SELECT TOP 1 [Value] FROM [Transient].[ConnectionVariable] WITH (NOLOCK) WHERE [ConnectionId]=@ConnectionId AND [Name]=@Name
END