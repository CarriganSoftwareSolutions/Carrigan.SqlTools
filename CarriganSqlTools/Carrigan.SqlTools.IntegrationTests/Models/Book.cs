using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.IntegrationTests.Models;

public sealed class Book
{
    [PrimaryKey]
    public int? Id { get; set; }
    public string? Title { get; set; } = string.Empty;
    public string? Author { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public int? Pages { get; set; }
    public int? YearPublished { get; set; }

    public static string CreateTableSqlServer => 
        """
        CREATE TABLE [Book]
        (
            [Id] INT NOT NULL,
            [Title] NVARCHAR(200) NOT NULL,
            [Author] NVARCHAR(150) NOT NULL,
            [Description] NVARCHAR(1000) NOT NULL,
            [Price] DECIMAL(10,2) NULL,
            [Pages] INT NOT NULL,
            [YearPublished] INT NOT NULL,
            CONSTRAINT [PK_Book] PRIMARY KEY ([Id])
        );
        """;

    public static string CreateTablePostgreSql =>
        """
        CREATE TABLE "Book"
        (
            "Id" INTEGER NOT NULL,
            "Title" VARCHAR(200) NOT NULL,
            "Author" VARCHAR(150) NOT NULL,
            "Description" VARCHAR(1000) NOT NULL,
            "Price" NUMERIC(10,2) NULL,
            "Pages" INTEGER NOT NULL,
            "YearPublished" INTEGER NOT NULL,
            CONSTRAINT "PK_Book" PRIMARY KEY ("Id")
        );
        """;
}
