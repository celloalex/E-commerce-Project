CREATE TABLE [dbo].[Purchase] (
    [ID]         INT IDENTITY (1, 1) NOT NULL,
    [CustomerID] INT NOT NULL,
    CONSTRAINT [PK_Purchase] PRIMARY KEY CLUSTERED ([ID] ASC),
	CONSTRAINT [FK_Purchase_Customer] FOREIGN KEY (CustomerID) REFERENCES Customer(Id)
);

