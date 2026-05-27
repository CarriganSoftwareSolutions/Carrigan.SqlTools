namespace Carrigan.SqlTools.IntegrationTests.Models;

//IGNORE SPELLING: Postgre

public sealed class Address
{
    public int Id { get; set; }
    public string StreetName { get; set; } = string.Empty;
    public int StreetNumber { get; set; }
    public string ZipCode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;

    public static string CreateTableSqlServer =>
        """
        CREATE TABLE [Address]
        (
            [Id] INT NOT NULL,
            [StreetName] NVARCHAR(100) NOT NULL,
            [StreetNumber] INT NOT NULL,
            [ZipCode] CHAR(5) NOT NULL,
            [City] NVARCHAR(100) NOT NULL,
            [State] NCHAR(2) NOT NULL,
            CONSTRAINT [PK_Address] PRIMARY KEY ([Id])
        );
        """;
    public static string CreateTablePostgreSql =>
        """
        CREATE TABLE "Address"
        (
            "Id" INTEGER NOT NULL,
            "StreetName" VARCHAR(100) NOT NULL,
            "StreetNumber" INTEGER NOT NULL,
            "ZipCode" CHAR(5) NOT NULL,
            "City" VARCHAR(100) NOT NULL,
            "State" CHAR(2) NOT NULL,
            CONSTRAINT "PK_Address" PRIMARY KEY ("Id")
        );
        """;
}
