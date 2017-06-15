CREATE TABLE [dbo].[Payment] (
    [ID]             INT            IDENTITY (1, 1) NOT NULL,
    [CreditCard]     INT            NULL,
    [CreditCardName] NVARCHAR (100) NULL,
    [CustomerID] INT NOT NULL, 
    CONSTRAINT [PK_Payment] PRIMARY KEY CLUSTERED ([ID] ASC), 
    CONSTRAINT [FK_Payment_Customer] FOREIGN KEY (CustomerID) REFERENCES Customer(ID)
);

