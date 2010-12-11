ALTER TABLE [dbo].[SqlConnectionNames]
    ADD CONSTRAINT [FK_SqlConnectionNames_Sections] FOREIGN KEY ([SectionId]) REFERENCES [dbo].[Sections] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

