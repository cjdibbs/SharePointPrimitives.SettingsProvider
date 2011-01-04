-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create TRIGGER [dbo].[SqlConnectionNames_Audit]
   ON  [dbo].SqlConnectionNames
   AFTER INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	
	update a 
	set Deleted = sysdatetime()
	from 
		audit.SqlConnectionNames a join 
		deleted d on a.Name = d.Name and a.SectionId = d.SectionId 
	where a.Deleted is null
	
	insert into audit.SqlConnectionNames (Created, Name, [SqlConnectionId], [SectionId])
	select sysdatetime(), i.Name, i.[SqlConnectionId], i.[SectionId] 
	from inserted as i
END