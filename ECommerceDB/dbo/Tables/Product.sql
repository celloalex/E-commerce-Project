CREATE TABLE [dbo].[Product] (
    [ID]              INT            IDENTITY (1, 1) NOT NULL,
    [Name]            NVARCHAR (100) NULL,
    [Price]           MONEY          NULL,
    [Quantity]        INT            NULL,
    [ProductTypeName] NVARCHAR (100) NOT NULL,
    [Image]           NVARCHAR (100) NULL,
    CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Product_ProductType] FOREIGN KEY ([ProductTypeName]) REFERENCES [dbo].[ProductType] ([Name])
);



