CREATE TABLE [audit].[ApplicationSettings] (
    [Created]   DATETIME       NOT NULL,
    [Deleted]   DATETIME       NULL,
    [SectionId] INT            NOT NULL,
    [Name]      VARCHAR (1024) NOT NULL,
    [Value]     VARCHAR (MAX)  NULL,
    [Type]      VARCHAR (MAX)  NULL
);

