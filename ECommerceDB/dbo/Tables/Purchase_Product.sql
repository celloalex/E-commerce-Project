CREATE TABLE [dbo].[Purchase_Product] (
    [PurchaseID] INT              NOT NULL,
    [ProductID]  INT              NOT NULL,
    [BundleID]   UNIQUEIDENTIFIER NULL,
    [Quantity]   INT              NULL,
    CONSTRAINT [PK_PurchaseProduct] PRIMARY KEY CLUSTERED ([PurchaseID] ASC, [ProductID] ASC),
    CONSTRAINT [FK_PurchaseProduct_Product] FOREIGN KEY ([ProductID]) REFERENCES [dbo].[Product] ([ID]),
    CONSTRAINT [FK_PurchaseProduct_Purchase] FOREIGN KEY ([PurchaseID]) REFERENCES [dbo].[Purchase] ([ID])
);

