namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Models;

public sealed class OrderedItem
{
    public int OrderId { get; set; }
    public int BookId { get; set; }
    public decimal Price { get; set; }

    public static string CreateTableSql => """
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
}
