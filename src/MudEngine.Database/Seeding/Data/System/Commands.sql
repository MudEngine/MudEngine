exec [System].[UpsertCommandAssembly] @CommandAssemblyId='ea0ad88f-fd70-46dc-b7ee-53f8b6339eb8', @Preload=1 -- OnNewConnection
exec [System].[UpsertCommandAssembly] @CommandAssemblyId='93c8ce1f-60bb-4a1c-9d38-fc8abea4756b', @Preload=1 -- Login
exec [System].[UpsertCommandAssembly] @CommandAssemblyId='20b61361-c4e3-425f-b50b-dcb4cd06c463', @Preload=1 -- Quit
exec [System].[UpsertCommandAssembly] @CommandAssemblyId='baa4ad89-36f7-4928-a59a-8ae620ec6564', @Preload=1 -- Unknown
exec [System].[UpsertCommandAssembly] @CommandAssemblyId='fc8b6041-ba8a-4c99-97f7-b01495eff307', @Preload=1 -- Look
exec [System].[UpsertCommandAssembly] @CommandAssemblyId='9d992665-662d-4730-9943-e89557ff946f', @Preload=1 -- Say
exec [System].[UpsertCommandAssembly] @CommandAssemblyId='a8b4ecd9-2c84-4a81-9c85-0b7458c24e26', @Preload=1 -- Whisper
exec [System].[UpsertCommandAssembly] @CommandAssemblyId='31c561d3-a60a-4383-a6d2-cb7bcdde4391', @Preload=1 -- Tell
exec [System].[UpsertCommandAssembly] @CommandAssemblyId='a211ca5b-aeef-461f-95fd-63bb3993e5a7', @Preload=1 -- Emote
exec [System].[UpsertCommandList] @Name='New Connections', @Priority=0
exec [System].[UpsertCommandListAssembly] @Name='New Connections', @CommandAssemblyId='20b61361-c4e3-425f-b50b-dcb4cd06c463', @PrimaryAlias='quit'
exec [System].[UpsertCommandListAssembly] @Name='New Connections', @CommandAssemblyId='93c8ce1f-60bb-4a1c-9d38-fc8abea4756b', @PrimaryAlias='login', @HandlesUnknown=1
exec [System].[UpsertCommandList] @Name='System', @Priority=0
exec [System].[UpsertCommandListAssembly] @Name='System', @CommandAssemblyId='1415a2de-1587-437f-8164-8a2843c1864f', @PrimaryAlias='move'
exec [System].[UpsertCommandList] @Name='Basic', @Priority=0
exec [System].[UpsertCommandListAssembly] @Name='Basic', @CommandAssemblyId='11ca3b8b-62d1-4811-ae6c-f6cd812c8e2c', @PrimaryAlias='motd'
exec [System].[UpsertCommandListAssembly] @Name='Basic', @CommandAssemblyId='fc8b6041-ba8a-4c99-97f7-b01495eff307', @PrimaryAlias='look'
exec [System].[UpsertCommandListAssembly] @Name='Basic', @CommandAssemblyId='9d992665-662d-4730-9943-e89557ff946f', @PrimaryAlias='say'
exec [System].[UpsertCommandListAssembly] @Name='Basic', @CommandAssemblyId='a8b4ecd9-2c84-4a81-9c85-0b7458c24e26', @PrimaryAlias='whisper'
exec [System].[UpsertCommandListAssembly] @Name='Basic', @CommandAssemblyId='31c561d3-a60a-4383-a6d2-cb7bcdde4391', @PrimaryAlias='tell'
exec [System].[UpsertCommandListAssembly] @Name='Basic', @CommandAssemblyId='a211ca5b-aeef-461f-95fd-63bb3993e5a7', @PrimaryAlias='emote'
exec [System].[UpsertCommandListAssembly] @Name='Basic', @CommandAssemblyId='20b61361-c4e3-425f-b50b-dcb4cd06c463', @PrimaryAlias='quit'
exec [System].[UpsertCommandListAssembly] @Name='Basic', @CommandAssemblyId='baa4ad89-36f7-4928-a59a-8ae620ec6564', @PrimaryAlias='unknown', @HandlesUnknown=1
