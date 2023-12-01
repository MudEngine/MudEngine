DECLARE @MudId int
DECLARE @ZoneId int
DECLARE @Room1Id int
DECLARE @Room2Id int
DECLARE @TestAccountId uniqueidentifier
DECLARE @TestPlayerId int
DECLARE @GuestAccountId uniqueidentifier
DECLARE @GuestPlayerId int

-- Create a Mud
EXEC @MudId = [System].[UpsertMud] @Name='Iconic: Capes ''n'' Criminals', @OnNewConnectionCommandId='ea0ad88f-fd70-46dc-b7ee-53f8b6339eb8'

-- Add a zone to the mud
EXEC @ZoneId = [System].[UpsertZone] @MudId, 'Test Zone'

-- Add a room to the Zone
EXEC @Room1Id = [System].[UpsertRoom] @ZoneId, 'Test Room', 'This is just a room.'
EXEC @Room2Id = [System].[UpsertRoom] @ZoneId, 'Test Room 2', 'This is another room.'

-- Add a test account with test password
EXEC [System].[CreateAccount] 'test', '$2a$11$kbmMMmYX1xMGfbnqxVewP.fJZKlK3LZsPQp1VnLqRzJ7BFF04mvVW'
SELECT @TestAccountId=AccountId FROM [System].[Account] WITH (NOLOCK) WHERE [Name]='test'

-- Add a test player to the account
EXEC @TestPlayerId = [System].[CreatePlayer] @TestAccountId, @Room1Id, 'Test'

-- Add a guest account with guest password
EXEC [System].[CreateAccount] 'guest', '$2a$11$I2g.aigSVAA7dzSOd6v1We5LKfUYEI75dIxLEl2Y2eGvGKjm8sFci'
SELECT @GuestAccountId=AccountId FROM [System].[Account] WITH (NOLOCK) WHERE [Name]='guest'

-- Add a guest player to the account
EXEC @GuestPlayerId = [System].[CreatePlayer] @GuestAccountId, @Room2Id, 'Guest'
