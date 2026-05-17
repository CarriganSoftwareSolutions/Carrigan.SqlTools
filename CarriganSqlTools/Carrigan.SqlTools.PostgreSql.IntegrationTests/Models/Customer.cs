using Carrigan.SqlTools.Attributes;
using System;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Models;

[Dialect(DialectEnum.PostgreSql)]
public sealed class Customer
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int Age { get; set; }
    public char Gender { get; set; }

    public static string CreateTableSql => 
        """
        CREATE TABLE "Customer"
        (
            "Id" INTEGER NOT NULL,
            "FirstName" VARCHAR(50) NOT NULL,
            "LastName" VARCHAR(50) NOT NULL,
            "Age" INTEGER NOT NULL,
            "Gender" CHAR(1) NOT NULL,
            CONSTRAINT "PK_Customer" PRIMARY KEY ("Id")
        );
        """;
}
