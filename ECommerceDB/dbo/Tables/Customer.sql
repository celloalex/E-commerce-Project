CREATE TABLE [dbo].[Customer] (
    [Id]    INT            IDENTITY (1, 1) NOT NULL,
    [Name]  NVARCHAR (100) NULL,
    [email] NVARCHAR (255) NULL,
    CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED ([Id])
);

