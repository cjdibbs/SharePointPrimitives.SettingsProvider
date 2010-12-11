ALTER DATABASE [$(DatabaseName)]
    ADD FILE (NAME = [SPPrimitives_SettingsProvider_Database], FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL10.MSSQLSERVER\MSSQL\DATA\SPPrimitives_SettingsProvider_Database.mdf', SIZE = 2304 KB, FILEGROWTH = 1024 KB) TO FILEGROUP [PRIMARY];

