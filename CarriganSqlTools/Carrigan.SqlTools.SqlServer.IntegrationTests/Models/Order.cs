using System;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Models;

public sealed class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int AddressId { get; set; }
    public DateOnly Date { get; set; }
    public decimal SalesTaxPercent { get; set; }

    public static string CreateTableSql =>
        """
        CREATE TABLE [Order]
        (
            [Id] INT NOT NULL,
            [CustomerId] INT NOT NULL,
            [AddressId] INT NOT NULL,
            [Date] DATE NOT NULL,
            [SalesTaxPercent] DECIMAL(7,4) NOT NULL,
            CONSTRAINT [PK_Orders] PRIMARY KEY ([Id]),
            CONSTRAINT [FK_Orders_Customer] FOREIGN KEY ([CustomerId]) REFERENCES [Customer] ([Id]),
            CONSTRAINT [FK_Orders_Address] FOREIGN KEY ([AddressId]) REFERENCES [Address] ([Id])
        );
        """;
}
