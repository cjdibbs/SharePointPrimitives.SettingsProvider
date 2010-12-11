ALTER TABLE [dbo].[SqlConnectionNames]
    ADD CONSTRAINT [FK_SqlConnectionNames_SqlConnectionStrings] FOREIGN KEY ([SqlConnectionId]) REFERENCES [dbo].[SqlConnectionStrings] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

