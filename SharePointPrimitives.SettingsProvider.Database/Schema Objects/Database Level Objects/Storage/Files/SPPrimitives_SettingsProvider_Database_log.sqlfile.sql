ALTER DATABASE [$(DatabaseName)]
    ADD LOG FILE (NAME = [SPPrimitives_SettingsProvider_Database_log], FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL10.MSSQLSERVER\MSSQL\DATA\SPPrimitives_SettingsProvider_Database_log.ldf', SIZE = 1024 KB, MAXSIZE = 2097152 MB, FILEGROWTH = 10 %);

