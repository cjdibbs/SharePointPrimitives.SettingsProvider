CREATE TABLE [dbo].[ApplicationSettings]
(
	SectionId int NOT NULL, 
	Name varchar(1024) not NULL,
	Value varchar(max),
	[Type] varchar(max)
)
