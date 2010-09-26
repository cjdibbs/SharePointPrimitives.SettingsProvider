CREATE TABLE [dbo].[SqlConnectionStrings] (
	Id int IDENTITY(1,1) not null,
	[Catalog] varchar(max) NOT NULL, 
	ConnectionString varchar(max) not null
)
