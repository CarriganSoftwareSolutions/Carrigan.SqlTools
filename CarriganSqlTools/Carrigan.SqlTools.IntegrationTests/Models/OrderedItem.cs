using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.IntegrationTests.Models;

public sealed class OrderedItem
{
    [PrimaryKey]
    public int? OrderId { get; set; }
    [PrimaryKey]
    public int? BookId { get; set; }
    public decimal? Price { get; set; }

    public static string CreateTableSqlServer => 
        """
        CREATE TABLE [OrderedItem]
        (
            [OrderId] INT NOT NULL,
            [BookId] INT NOT NULL,
            [Price] DECIMAL(10,2) NOT NULL,
            CONSTRAINT [PK_OrderedItem] PRIMARY KEY ([OrderId], [BookId]),
            CONSTRAINT [FK_OrderedItem_Orders] FOREIGN KEY ([OrderId]) REFERENCES [Order] ([Id]),
            CONSTRAINT [FK_OrderedItem_Book] FOREIGN KEY ([BookId]) REFERENCES [Book] ([Id])
        );
        """;

    public static string CreateTablePostgreSql =>
        """
        CREATE TABLE "OrderedItem"
        (
            "OrderId" INTEGER NOT NULL,
            "BookId" INTEGER NOT NULL,
            "Price" NUMERIC(10,2) NOT NULL,
            CONSTRAINT "PK_OrderedItem" PRIMARY KEY ("OrderId", "BookId"),
            CONSTRAINT "FK_OrderedItem_Orders" FOREIGN KEY ("OrderId") REFERENCES "Order" ("Id"),
            CONSTRAINT "FK_OrderedItem_Book" FOREIGN KEY ("BookId") REFERENCES "Book" ("Id")
        );
        """;
}
