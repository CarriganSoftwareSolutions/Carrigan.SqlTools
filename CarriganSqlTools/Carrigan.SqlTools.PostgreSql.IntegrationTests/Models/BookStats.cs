using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Models;

[Dialect(DialectEnum.PostgreSql)]
public sealed class BookStats
{
    public int BookId { get; set; }
    public int StockCount { get; set; }
    public decimal AverageReview { get; set; }

    public static string CreateTableSql =>
        """
        CREATE TABLE "BookStats"
        (
            "BookId" INTEGER NOT NULL,
            "StockCount" INTEGER NOT NULL,
            "AverageReview" NUMERIC(10,2) NOT NULL,
            CONSTRAINT "PK_BookStats" PRIMARY KEY ("BookId"),
            CONSTRAINT "FK_BookStats_Book" FOREIGN KEY ("BookId") REFERENCES "Book" ("Id")
        );
        """;
}
