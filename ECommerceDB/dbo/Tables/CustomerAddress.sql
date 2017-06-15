CREATE TABLE [dbo].[CustomerAddress] (
    [CustomerID] INT NOT NULL,
    [AddressID]  INT NOT NULL,
    CONSTRAINT [PK_CustomerAddress] PRIMARY KEY CLUSTERED ([CustomerID] ASC, [AddressID] ASC),
    CONSTRAINT [FK_CustomerAddress_Address] FOREIGN KEY ([AddressID]) REFERENCES [dbo].[Address] ([ID]),
    CONSTRAINT [FK_CustomerAddress_Customer] FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[Customer] ([Id])
);

