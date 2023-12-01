DECLARE @SqlVersion nvarchar(78) = CONVERT(nvarchar, SERVERPROPERTY('productversion'))
DECLARE @SqlVersionMajor nvarchar(78) = SUBSTRING(@SqlVersion, 0, CHARINDEX('.', @SqlVersion))
DECLARE @MemFiles int = 0
IF CONVERT(int, @SqlVersionMajor) > 12
	BEGIN
		DECLARE @CompatibilityLevel int
		DECLARE @MemoryOptimized bit=0
		DECLARE @MemFileGroups int
		SELECT @CompatibilityLevel=[compatibility_level] FROM sys.databases WHERE [name] = Db_Name()
		IF @CompatibilityLevel < @SqlVersionMajor * 10
			BEGIN
				DECLARE @Compat nvarchar(250) = 'ALTER DATABASE ' + Db_Name() + ' SET COMPATIBILITY_LEVEL = ' + @SqlVersionMajor + '0;'
				EXEC sp_executesql @Compat
				SELECT @CompatibilityLevel=[compatibility_level] FROM sys.databases WHERE [name] = Db_Name()
			END
		IF @CompatibilityLevel >= 130
			BEGIN
				SELECT @MemoryOptimized=[is_memory_optimized_elevate_to_snapshot_on] FROM sys.databases WHERE [name] = Db_Name()
				IF @MemoryOptimized=0
					BEGIN
						DECLARE @MemOpt nvarchar(250) = 'ALTER DATABASE ' + Db_Name() + ' SET MEMORY_OPTIMIZED_ELEVATE_TO_SNAPSHOT = ON;'
						EXEC sp_executesql @MemOpt
						SELECT @MemoryOptimized=[is_memory_optimized_elevate_to_snapshot_on] FROM sys.databases WHERE [name] = Db_Name()
					END
			END
		IF @MemoryOptimized = 1
			BEGIN
				SELECT @MemFileGroups=COUNT(*) FROM sys.filegroups WHERE PatIndex('MEMORY_OPTIMIZED%', CONVERT(nvarchar, [type_desc])) > 0
				IF @MemFileGroups=0
					BEGIN
						DECLARE @MemFileGroup nvarchar(250) = 'ALTER DATABASE ' + Db_Name() + ' ADD FILEGROUP ' + Db_Name() + '_mod CONTAINS MEMORY_OPTIMIZED_DATA;'
						EXEC sp_executesql @MemFileGroup
						SELECT @MemFileGroups=COUNT(*) FROM sys.filegroups WHERE PatIndex('MEMORY_OPTIMIZED%', CONVERT(nvarchar, [type_desc])) > 0
					END
			END
		IF @MemFileGroups > 0
			BEGIN
				SELECT @MemFiles=COUNT(*) FROM sys.database_files [databasefile] INNER JOIN sys.filegroups [filegroup] ON [databasefile].data_space_id = [filegroup].data_space_id WHERE PatIndex('MEMORY_OPTIMIZED%', CONVERT(nvarchar, [filegroup].[type_desc])) > 0
				IF @MemFiles=0
					BEGIN
						DECLARE @PhysicalName nvarchar(250)
						SELECT TOP 1 @PhysicalName=[physical_name] FROM sys.master_files WHERE database_id=DB_ID() AND type_desc='ROWS'
						IF(CHARINDEX('\', @PhysicalName) > 0)
							BEGIN
								DECLARE @PhysicalPath nvarchar(250) = LEFT(@PhysicalName,LEN(@PhysicalName) - charindex('\',reverse(@PhysicalName),1) + 1)
								DECLARE @MemFile nvarchar(250) = 'ALTER DATABASE ' + Db_Name() + ' ADD FILE (name=''' + Db_Name() + '_mod1'', filename=''' + @PhysicalPath + Db_Name() + ''') TO FILEGROUP ' + Db_Name() + '_mod;'
								EXEC sp_executesql @MemFile
								SELECT @MemFiles=COUNT(*) FROM sys.database_files [databasefile] INNER JOIN sys.filegroups [filegroup] ON [databasefile].data_space_id = [filegroup].data_space_id WHERE PatIndex('MEMORY_OPTIMIZED%', CONVERT(nvarchar, [filegroup].[type_desc])) > 0
							END
					END
			END
	END