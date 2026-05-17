using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Models;

[Dialect(DialectEnum.PostgreSql)]
public sealed class Address
{
    public int Id { get; set; }
    public string StreetName { get; set; } = string.Empty;
    public int StreetNumber { get; set; }
    public string Zipcode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;

    public static string CreateTableSql => 
        """
        CREATE TABLE "Address"
        (
            "Id" INTEGER NOT NULL,
            "StreetName" VARCHAR(100) NOT NULL,
            "StreetNumber" INTEGER NOT NULL,
            "Zipcode" CHAR(5) NOT NULL,
            "City" VARCHAR(100) NOT NULL,
            "State" CHAR(2) NOT NULL,
            CONSTRAINT "PK_Address" PRIMARY KEY ("Id")
        );
        """;
}
