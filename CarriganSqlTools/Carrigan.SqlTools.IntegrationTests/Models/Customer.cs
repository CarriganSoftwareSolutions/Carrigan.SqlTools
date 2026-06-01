using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.IntegrationTests.Models;

public sealed class Customer
{
    [PrimaryKey]
    public int? Id { get; set; }
    public string? FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; } = string.Empty;
    public int? Age { get; set; }
    public char? Gender { get; set; }

    public static string CreateTableSqlServer => 
        """
        CREATE TABLE [Customer]
        (
            [Id] INT NOT NULL,
            [FirstName] NVARCHAR(50) NOT NULL,
            [LastName] NVARCHAR(50) NOT NULL,
            [Age] INT NOT NULL,
            [Gender] NCHAR(1) NOT NULL,
            CONSTRAINT [PK_Customer] PRIMARY KEY ([Id])
        );
        """;

    public static string CreateTablePostgreSql =>
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
