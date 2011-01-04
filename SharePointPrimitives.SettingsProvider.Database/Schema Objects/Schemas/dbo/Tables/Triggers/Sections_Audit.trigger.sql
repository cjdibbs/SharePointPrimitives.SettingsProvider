-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE TRIGGER [dbo].[Sections_Audit]
   ON  [dbo].[Sections]
   AFTER INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	
	update a
	set a.Deleted = sysdatetime()
	from  audit.[Sections] a join deleted d on d.Id = a.Id
	where a.Deleted is null
	
	insert into audit.[Sections] (Created, Id, Name, AssemblyName)
	select sysdatetime(), i.Id, i.Name, i.AssemblyName 
	from inserted as i
END