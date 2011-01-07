-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [reload_from_audit] 
	@pointInTime datetime
AS
BEGIN
begin transaction

	delete from SqlConnectionNames
	delete from SqlConnectionStrings
	delete from ApplicationSettings
	delete from Sections

	set IDENTITY_INSERT dbo.Sections on
	insert into dbo.Sections (Id, Name, AssemblyName)
	select Id, Name, AssemblyName
	from audit.Sections
	where Created < @pointInTime and Deleted > @pointInTime
	set IDENTITY_INSERT dbo.Sections off

	insert into dbo.ApplicationSettings (SectionId, Name, Value, [Type])
	select SectionId, Name, Value, [Type]
	from audit.ApplicationSettings
	where Created < @pointInTime and Deleted > @pointInTime

	set IDENTITY_INSERT dbo.SqlConnectionStrings on
	insert into dbo.SqlConnectionStrings(Id, ConnectionString)
	select Id, ConnectionString
	from audit.SqlConnectionStrings
	where Created < @pointInTime and Deleted > @pointInTime
	set IDENTITY_INSERT dbo.SqlConnectionStrings off

	insert into dbo.SqlConnectionNames (SectionId, Name, SqlConnectionId)
	select SectionId, Name, SqlConnectionId
	from audit.SqlConnectionNames
	where Created < @pointInTime and Deleted > @pointInTime


commit
END