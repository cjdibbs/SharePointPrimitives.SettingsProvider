ALTER TABLE [dbo].[ApplicationSettings]
    ADD CONSTRAINT [FK_ApplcationSettings_Sections] FOREIGN KEY ([SectionId]) REFERENCES [dbo].[Sections] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

