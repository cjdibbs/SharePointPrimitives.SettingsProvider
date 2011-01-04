-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create TRIGGER [dbo].[SqlConnectionStrings_Audit]
   ON  [dbo].[SqlConnectionStrings]
   AFTER INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	
	update a 
	set Deleted = sysdatetime()
	from 
		audit.[SqlConnectionStrings] a join 
		deleted d on a.Id = d.Id
	where a.Deleted is null
	
	insert into audit.[SqlConnectionStrings] (Created, Id, [ConnectionString])
	select sysdatetime(), i.Id, i.[ConnectionString] 
	from inserted as i
END