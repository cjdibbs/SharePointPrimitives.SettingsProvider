ALTER TABLE [dbo].[SqlConnectionNames]
	ADD CONSTRAINT [SqlConnection_SqlConnectionNames_FK] 
	FOREIGN KEY (SqlConnectionId)
	REFERENCES SqlConnectionStrings (Id)	

