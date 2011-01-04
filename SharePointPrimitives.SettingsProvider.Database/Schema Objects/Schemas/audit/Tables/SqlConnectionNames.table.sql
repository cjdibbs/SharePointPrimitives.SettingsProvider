CREATE TABLE [audit].[SqlConnectionNames] (
    [Created]         DATETIME       NOT NULL,
    [Deleted]         DATETIME       NULL,
    [Name]            VARCHAR (1024) NOT NULL,
    [SqlConnectionId] INT            NOT NULL,
    [SectionId]       INT            NOT NULL
);

