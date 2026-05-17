using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Models;

[Dialect(DialectEnum.PostgreSql)]
public sealed class OrderedItem
{
    public int OrderId { get; set; }
    public int BookId { get; set; }
    public decimal Price { get; set; }

    public static string CreateTableSql => 
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
