namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Models;

public sealed class BookStats
{
    public int BookId { get; set; }
    public int StockCount { get; set; }
    public float AverageReview { get; set; }

    public static string CreateTableSql => 
        """
        CREATE TABLE "BookStats"
        (
            "BookId" INTEGER NOT NULL,
            "StockCount" INTEGER NOT NULL,
            "AverageReview" REAL NOT NULL,
            CONSTRAINT "PK_BookStats" PRIMARY KEY ("BookId"),
            CONSTRAINT "FK_BookStats_Book" FOREIGN KEY ("BookId") REFERENCES "Book" ("Id")
        );
        """;
}
