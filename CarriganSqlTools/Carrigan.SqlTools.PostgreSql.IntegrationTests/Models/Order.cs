using Carrigan.SqlTools.Attributes;
using System;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Models;

[Dialect(DialectEnum.PostgreSql)]
public sealed class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int AddressId { get; set; }
    public DateOnly Date { get; set; }
    public decimal SalesTaxPercent { get; set; }

    public static string CreateTableSql => 
        """
        CREATE TABLE "Order"
        (
            "Id" INTEGER NOT NULL,
            "CustomerId" INTEGER NOT NULL,
            "AddressId" INTEGER NOT NULL,
            "Date" DATE NOT NULL,
            "SalesTaxPercent" NUMERIC(7,4) NOT NULL,
            CONSTRAINT "PK_Orders" PRIMARY KEY ("Id"),
            CONSTRAINT "FK_Orders_Customer" FOREIGN KEY ("CustomerId") REFERENCES "Customer" ("Id"),
            CONSTRAINT "FK_Orders_Address" FOREIGN KEY ("AddressId") REFERENCES "Address" ("Id")
        );
        """;
}
