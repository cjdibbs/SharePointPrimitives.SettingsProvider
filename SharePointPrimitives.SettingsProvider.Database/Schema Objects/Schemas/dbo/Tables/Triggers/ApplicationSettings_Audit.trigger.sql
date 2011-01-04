-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create TRIGGER [dbo].[ApplicationSettings_Audit]
   ON  [dbo].ApplicationSettings
   AFTER INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	
	update a 
	set Deleted = sysdatetime()
	from 
		audit.ApplicationSettings a join 
		deleted d on a.Name = d.Name and a.SectionId = d.SectionId 
	where a.Deleted is null
	
	insert into audit.ApplicationSettings (Created, SectionId, Name, Value, [Type])
	select sysdatetime(), i.SectionId, i.Name, i.Value, i.[Type] 
	from inserted as i
END