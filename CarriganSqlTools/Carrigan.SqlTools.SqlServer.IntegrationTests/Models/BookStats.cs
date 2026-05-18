namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Models;

public sealed class BookStats
{
    public int BookId { get; set; }
    public int StockCount { get; set; }
    public decimal AverageReview { get; set; }

    public static string CreateTableSql =>
        """
        CREATE TABLE [BookStats]
        (
            [BookId] INT NOT NULL,
            [StockCount] INT NOT NULL,
            [AverageReview] DECIMAL(10,2) NOT NULL,
            CONSTRAINT [PK_BookStats] PRIMARY KEY ([BookId]),
            CONSTRAINT [FK_BookStats_Book] FOREIGN KEY ([BookId]) REFERENCES [Book] ([Id])
        );
        """;
}
