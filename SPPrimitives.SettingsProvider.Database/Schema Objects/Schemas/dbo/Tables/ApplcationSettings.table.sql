CREATE TABLE [dbo].[ApplcationSettings]
(
	SectionId int NOT NULL, 
	Name varchar(1024) not NULL,
	SerializeAs int not null,
	Value varchar(max)
)
