CREATE TABLE [dbo].[Address] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [StreetLine1] NVARCHAR (100) NULL,
    [StreetLine2] NVARCHAR (100) NULL,
    [City]        NVARCHAR (100) NULL,
    [State]       NVARCHAR (50)  NULL,
    [Zip]         NVARCHAR (12)  NULL,
    CONSTRAINT [PK_Address] PRIMARY KEY CLUSTERED ([ID] ASC)
);

